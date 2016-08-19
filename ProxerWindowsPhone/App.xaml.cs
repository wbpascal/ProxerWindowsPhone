using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Proxer
{
    public sealed partial class App : Application
    {
        private TransitionCollection _transitions;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
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
                    {
                        this._transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            Window.Current.Activate();
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
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