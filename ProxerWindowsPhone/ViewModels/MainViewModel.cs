using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Azuria.Exceptions;
using Proxer.Utility;
using Proxer.ViewModels.Media;
using Proxer.Views.Media;
using ReactiveUI;

namespace Proxer.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private bool _isLoading;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        #region Properties

        public bool IsLoading
        {
            get { return this._isLoading; }
            set { this.RaiseAndSetIfChanged(ref this._isLoading, value); }
        }

        #endregion

        #region Methods

        public bool CancelAllTasks()
        {
            if (this._tokenSource.IsCancellationRequested) return false;

            this._tokenSource.Cancel();
            return true;
        }

        private void CreateCancellationToken()
        {
            if (this._tokenSource.IsCancellationRequested) this._tokenSource = new CancellationTokenSource();
        }

        public async Task HandleUri(Uri uri)
        {
            this.CreateCancellationToken();
            this.IsLoading = true;

            try
            {
                if (uri.Authority.Equals("proxer.me"))
                    await ShowReaderDialog(uri, this._tokenSource.Token).ConfigureAwait(true);
                else
                    await MediaHandler.HandleStreamPartnerUri(uri, this._tokenSource.Token).ConfigureAwait(true);
            }
            catch (TaskCanceledException)
            {
                //ignored as it is intended
            }
            catch
            {
                MessageQueue.AddMessage(ResourceUtility.GetString("LoadingErrorMsg"));
            }

            this.IsLoading = false;
        }

        private static async Task NavigateToInternalReader(Uri uri, CancellationToken token)
        {
            try
            {
                MangaReaderViewModel lViewModel = await MangaReaderViewModel.Create(uri).ConfigureAwait(true);
                if (lViewModel == null)
                {
                    MessageQueue.AddMessage(ResourceUtility.GetString("RedirectToProxerReaderMsg"));
                    NavigationHelper.NavigateToUrl(uri.AddQueryParam("wp_skip", "true"));
                }
                else
                {
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame?.Navigate(lViewModel.IsSlide
                        ? typeof(SlideMangaReaderView)
                        : typeof(VerticalMangaReaderView), lViewModel);
                }
            }
            catch (TaskCanceledException)
            {
                //ignored as it is intended
            }
            catch (CaptchaException)
            {
                NavigationHelper.NavigateToUrl(new Uri("https://proxer.me/misc/captcha"));
            }
            catch
            {
                MessageQueue.AddMessage(ResourceUtility.GetString("LoadingErrorMsg"));
            }
        }

        public bool ShouldHandleUri(Uri uri)
        {
            if (uri.Query.Contains("wp_skip=true")) return false;
            return !uri.Authority.Equals("proxer.me") || MediaHandler.MangaUriMatch(uri).Success;
        }

        private static async Task ShowReaderDialog(Uri uri, CancellationToken token)
        {
            MessageDialog lMessageDialog =
                new MessageDialog(ResourceUtility.GetString("ChooseReaderMsg"));
            lMessageDialog.Commands.Add(new UICommand(ResourceUtility.GetString("InternalMangaReaderChoice"),
                async command => await NavigateToInternalReader(uri, token).ConfigureAwait(true)));
            lMessageDialog.Commands.Add(new UICommand(ResourceUtility.GetString("OfficialMangaReaderChoice"),
                command => NavigationHelper.NavigateToUrl(uri.AddQueryParam("wp_skip", "true"))));
            await lMessageDialog.ShowAsync();
        }

        #endregion
    }
}