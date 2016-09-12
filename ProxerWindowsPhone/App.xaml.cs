using System;
using System.Reflection;
using System.Threading;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Proxer.Utility;

namespace Proxer
{
    public sealed partial class App
    {
        private TransitionCollection _transitions;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;
        }

        #region

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame {CacheSize = 1};
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                if (rootFrame.ContentTransitions != null)
                {
                    this._transitions = new TransitionCollection();
                    foreach (Transition c in rootFrame.ContentTransitions)
                        this._transitions.Add(c);
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                    throw new Exception("Failed to create initial page");
            }

            Window.Current.Activate();
        }

        private static void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        private static async void OnUnhandledException(object sender,
            UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            unhandledExceptionEventArgs.Handled = true;
            Exception lException = unhandledExceptionEventArgs.Exception;
            try
            {
                await
                    HttpUtility.GetRequest(
                        new Uri(Uri.EscapeUriString("https://piwik.infinitesoul.me/piwik.php?idsite=1&rec=1" +
                                                    "&url=https://error.infinitesoul.me/&action_name=ErrorReport&apiv=1" +
                                                    $"&c_n={lException.GetType().FullName}" +
                                                    $"&c_p={{\"version\": \"{typeof(App).GetTypeInfo().Assembly.GetName().Version}\", " +
                                                    $"\"unhandledMessage\": \"{unhandledExceptionEventArgs.Message}\", " +
                                                    $"\"exceptionMessage\": \"{lException.Message}\", " +
                                                    $"\"stacktrace\": \"{lException.StackTrace}\"}}" +
                                                    "&c_i=&send_image=0")),
                        CancellationToken.None);
            }
            catch
            {
                //ignore
            }
            finally
            {
                await MessageQueue.CancelShowMessages();
                await
                    new MessageDialog(
                        "Es ist ein Fehler aufgetreten und die Anwendung kann nicht fortfahren! " +
                        "Der Fehler wurde gemeldet und die Anwendung wird nun heruntergefahren!", "ERROR!").ShowAsync();
                Current.Exit();
            }
        }

        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            Frame rootFrame = sender as Frame;
            if (rootFrame == null) return;
            rootFrame.ContentTransitions = this._transitions ??
                                           new TransitionCollection {new NavigationThemeTransition()};
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        #endregion
    }
}