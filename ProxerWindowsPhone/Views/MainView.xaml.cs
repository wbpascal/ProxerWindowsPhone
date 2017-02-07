using System;
using System.Threading.Tasks;
using Windows.Data.Html;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web;
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
            if (backPressedEventArgs.Handled || !this.MainWebView.CanGoBack) return;
            backPressedEventArgs.Handled = true;
            this.MainWebView.GoBack();
        }

        private async void MainWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (!args.IsSuccess) await this.ShowRetryDialog(args.Uri, args.WebErrorStatus);

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

        private async Task ShowRetryDialog(Uri url, WebErrorStatus errorCode)
        {
            MessageDialog lRetryDialog =
                new MessageDialog(string.Format(ResourceUtility.GetString("CouldNotNavigateMsg"), errorCode));
            lRetryDialog.Commands.Add(new UICommand(ResourceUtility.GetString("RetryNavigationChoice"),
                command => this.MainWebView.Navigate(url)));
            lRetryDialog.Commands.Add(new UICommand(ResourceUtility.GetString("CancelNavigationChoice"),
                command => Application.Current.Exit()));
            await lRetryDialog.ShowAsync();
        }

        public void NavigateToUrl(Uri url)
        {
            this.MainWebView.Navigate(url);
        }

#pragma warning disable 4014
        private void MainWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (!this.ViewModel.ShouldHandleUri(args.Uri)) return;
            args.Cancel = true;
            this.ViewModel.HandleUri(args.Uri);
        }
#pragma warning restore 4014

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