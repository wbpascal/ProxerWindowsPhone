﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Proxer.Utility;
using Proxer.ViewModels.Media;
using Proxer.Views.Media;
using ReactiveUI;

namespace Proxer.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private bool _isLoading;

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

        public async Task HandleUri(Uri uri)
        {
            this.IsLoading = true;

            try
            {
                if (uri.Authority.Equals("proxer.me"))
                    NavigateToReader(await MangaReaderViewModel.Create(uri).ConfigureAwait(true));
                else
                    await MediaHandler.HandleStreamPartnerUri(uri, this._tokenSource.Token).ConfigureAwait(true);
            }
            catch (TaskCanceledException)
            {
                //ignored as it is intended
            }
            catch (Exception ex)
            {
                MessageQueue.AddMessage("Beim Laden ist ein Fehler aufgetreten! Bitte probiere es später erneut!");
            }

            this.IsLoading = false;
        }

        private static void NavigateToReader(MangaReaderViewModel viewModel)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame?.Navigate(viewModel.IsSlide
                ? typeof(SlideMangaReaderView)
                : typeof(VerticalMangaReaderView), viewModel);
        }

        public bool ShouldHandleUriInternal(Uri uri)
        {
            return !uri.Authority.Equals("proxer.me") || MediaHandler.MangaUriMatch(uri).Success;
        }

        #endregion
    }
}