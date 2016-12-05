using System;
using System.Threading;
using System.Threading.Tasks;
using Proxer.Utility;
using ReactiveUI;

namespace Proxer.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private Uri _currentBrowserUri;

        private bool _isLoadingStream;

        #region Properties

        public Uri CurrentBrowserUri
        {
            get { return this._currentBrowserUri; }
            set { this.RaiseAndSetIfChanged(ref this._currentBrowserUri, value); }
        }

        public bool IsLoadingStream
        {
            get { return this._isLoadingStream; }
            set { this.RaiseAndSetIfChanged(ref this._isLoadingStream, value); }
        }

        #endregion

        #region Methods

        public bool CancelAllTasks()
        {
            if (this._tokenSource.IsCancellationRequested) return false;

            this._tokenSource.Cancel();
            return true;
        }

        public async Task<bool> HandleUri(Uri uri)
        {
            if (uri.Authority.Equals("proxer.me"))
                return MediaHandler.HandleMangaReaderUri(uri);
            this.IsLoadingStream = true;
            try
            {
                await MediaHandler.HandleStreamPartnerUri(uri, this._tokenSource.Token).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                //ignored as it is intended
            }
            catch
            {
                MessageQueue.AddMessage("Der Stream konnte nicht abgerufen werden!");
            }
            finally
            {
                this.IsLoadingStream = false;
            }
            return true;
        }

        #endregion
    }
}