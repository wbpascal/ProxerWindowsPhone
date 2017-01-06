using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Azuria.Api;
using Newtonsoft.Json;
using Proxer.Utility;
using Proxer.Views;

namespace Proxer
{
    public sealed partial class App
    {
        private TransitionCollection _transitions;

        public App()
        {
            HardwareButtons.BackPressed += HardwareButtonsOnBackPressed;
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;
        }

        #region Methods

        private static void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs backPressedEventArgs)
        {
            Frame lRootFrame = Window.Current.Content as Frame;

            if ((lRootFrame == null) || !lRootFrame.CanGoBack) return;
            backPressedEventArgs.Handled = true;
            lRootFrame.GoBack();
        }

        #endregion

        #region

        private static void LoadSecrets()
        {
            const string lResourceName = "Proxer.secrets.json";
            Assembly lAssembly = typeof(App).GetTypeInfo().Assembly;

            using (Stream stream = lAssembly.GetManifestResourceStream(lResourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    Dictionary<string, string> lSecrets =
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.ReadToEnd());
                    ApiInfo.Init(input => input.ApiKeyV1 = lSecrets["apiKey"].ToCharArray());
                }
            }
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            MessageQueue.Initialise(TaskScheduler.FromCurrentSynchronizationContext());
            LoadSecrets();

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

                if (!rootFrame.Navigate(typeof(MainView), e.Arguments))
                    throw new Exception("Failed to create initial page");
            }

            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseVisible);
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
            try
            {
#if !DEBUG
                await PiwikLogger.LogUnhandledException(unhandledExceptionEventArgs, CancellationToken.None)
                    .ConfigureAwait(false);
                #endif
            }
            catch
            {
                // ignored
            }
            finally
            {
                await MessageQueue.CancelQueue().ConfigureAwait(false);
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