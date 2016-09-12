using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Html;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Proxer.Utility;

namespace Proxer
{
    public sealed partial class MainPage
    {
        private CancellationTokenSource _tokenSource;

        public MainPage()
        {
            MessageQueue.CurrentTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
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
            if (backPressedEventArgs.Handled || ((Window.Current.Content as Frame)?.SourcePageType != typeof(MainPage)))
                return;

            backPressedEventArgs.Handled = true;
            if ((this._tokenSource != null) && !this._tokenSource.IsCancellationRequested) this._tokenSource.Cancel();
            else if (this.MainWebView.CanGoBack) this.MainWebView.GoBack();
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
            this.IsLoadingStream = true;
            try
            {
                await
                    VideoUriFetcher.HandleStreamPartnerUri(args.Uri,
                        (this._tokenSource = new CancellationTokenSource()).Token);
            }
            catch (TaskCanceledException)
            {
                //ignored
            }
            catch
            {
                MessageQueue.AddMessage("Der Stream konnte nicht abgerufen werden!");
            }
            finally
            {
                this.IsLoadingStream = false;
            }
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