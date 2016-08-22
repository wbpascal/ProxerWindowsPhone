using System;
using Windows.Data.Html;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Proxer
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            HardwareButtons.BackPressed += this.HardwareButtonsOnBackPressed;
        }

        #region Properties

        private bool IsLoadingStream
        {
            set { this.LoadingSignGrid.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        #endregion

        #region

        private void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs backPressedEventArgs)
        {
            if (backPressedEventArgs.Handled || (Window.Current.Content as Frame)?.SourcePageType != typeof(MainPage))
                return;

            backPressedEventArgs.Handled = true;
            if (this.MainWebView.CanGoBack) this.MainWebView.GoBack();
            else Application.Current.Exit();
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
            if (args.Uri.Authority.Equals("proxer.me")) return;

            args.Cancel = true;
            try
            {
                this.IsLoadingStream = true;
                await VideoUriFetcher.HandleStreamPartnerUri(args.Uri);
                this.IsLoadingStream = false;
            }
            catch
            {
                await new MessageDialog("Der Stream konnte nicht abgerufen werden!").ShowAsync();
            }
        }

        private async void MainWebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            await new MessageDialog(HtmlUtilities.ConvertToText(e.Value)).ShowAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!this.MainWebView.CanGoBack) this.MainWebView.Navigate(new Uri("https://proxer.me/?device=mobile"));
        }

        #endregion
    }
}