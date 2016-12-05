using System;
using Windows.Data.Html;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Proxer.Utility;
using Proxer.ViewModels;

namespace Proxer.Views
{
    public sealed partial class MainView
    {
        public MainView()
        {
            this.DataContext = this;
            this.ViewModel = new MainViewModel();
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            HardwareButtons.BackPressed += this.HardwareButtonsOnBackPressed;
        }

        #region Properties

        public MainViewModel ViewModel { get; }

        #endregion

        #region

        private void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs backPressedEventArgs)
        {
            if (backPressedEventArgs.Handled || this.ViewModel.CancelAllTasks() || !this.MainWebView.CanGoBack) return;
            backPressedEventArgs.Handled = true;
            this.MainWebView.GoBack();
        }

        private async void MainWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            string[] code = {"window.alert = function(message) { window.external.notify(message) };"};
            try
            {
                await sender.InvokeScriptAsync("eval", code);
            }
            catch
            {
                //ignored
            }
        }

        private async void MainWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            args.Cancel = await this.ViewModel.HandleUri(args.Uri).ConfigureAwait(false);
        }

        private void MainWebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            MessageQueue.AddMessage(HtmlUtilities.ConvertToText(e.Value));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!this.MainWebView.CanGoBack) this.MainWebView.Navigate(new Uri("https://proxer.me/?device=mobile"));
        }

        #endregion
    }
}