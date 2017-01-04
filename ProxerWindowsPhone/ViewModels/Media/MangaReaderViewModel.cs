using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Azuria.ErrorHandling;
using Azuria.Exceptions;
using Azuria.Media;
using Azuria.Media.Properties;
using Proxer.Enumerable;
using Proxer.Utility;
using ReactiveUI;
using Splat;

namespace Proxer.ViewModels.Media
{
    public class MangaReaderViewModel : ReactiveObject
    {
        private readonly DispatcherTimer _containerFadeTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };

        private int _currentPageIndex;
        private bool _isContainerVisible = true;
        private bool _isLoadingPages;

        public MangaReaderViewModel(Manga.Chapter chapter, MangaPageEnumerable pageEnumerable, bool isSlide)
        {
            this.PageEnumerable = pageEnumerable;
            this.Chapter = chapter;
            this.IsSlide = isSlide;

            this.NavigateBackCommand = ReactiveCommand.Create(NavigationHelper.NavigateBack);
            this._containerFadeTimer.Tick += this.ContainerFadeTimerOnTick;
            this._containerFadeTimer.Start();
        }

        #region Properties

        public Manga.Chapter Chapter { get; }

        public int CurrentPageIndex
        {
            get { return this._currentPageIndex; }
            set { this.RaiseAndSetIfChanged(ref this._currentPageIndex, value); }
        }

        public bool IsContainerVisible
        {
            get { return this._isContainerVisible; }
            set { this.RaiseAndSetIfChanged(ref this._isContainerVisible, value); }
        }

        public bool IsLoadingPages
        {
            get { return this._isLoadingPages; }
            set { this.RaiseAndSetIfChanged(ref this._isLoadingPages, value); }
        }

        public bool IsSlide { get; }

        public ReactiveCommand NavigateBackCommand { get; }

        public MangaPageEnumerable PageEnumerable { get; }

        public ReactiveList<BitmapSource> PageImages { get; } = new ReactiveList<BitmapSource>();

        #endregion

        #region Methods

        private void ContainerFadeTimerOnTick(object sender, object o)
        {
            if (!this.IsLoadingPages) this.IsContainerVisible = false;
            this._containerFadeTimer.Stop();
        }

        public static async Task<MangaReaderViewModel> Create(Uri uri)
        {
            Match lMatch = MediaHandler.MangaUriMatch(uri);
            if (!lMatch.Success) return null;

            int lMangaId = Convert.ToInt32(lMatch.Groups["manga_id"].Value);
            int lChapterNr = Convert.ToInt32(lMatch.Groups["chapter_id"].Value);
            Language lLanguage = LanguageUtility.GetFromChapterString(lMatch.Groups["lang"].Value);
            bool lIsSlide = uri.Query.Contains("v=slide_beta");

            IProxerResult<IMediaObject> lMangaResult = await MediaObject.CreateFromId(lMangaId).ConfigureAwait(true);
            Manga lManga = lMangaResult.Result as Manga;
            if (!lMangaResult.Success || (lManga == null))
            {
                Exception lCaptchaException = lMangaResult.Exceptions.FirstOrDefault(
                    exception => exception is CaptchaException);
                if (lCaptchaException == null) return null;
                throw lCaptchaException;
            }

            IProxerResult<IEnumerable<Manga.Chapter>> lChaptersResult =
                await lManga.GetChapters(lLanguage).ConfigureAwait(true);
            if (!lChaptersResult.Success || (lChaptersResult.Result == null)) return null;
            Manga.Chapter lChapter = lChaptersResult.Result.FirstOrDefault(chapter => chapter.ContentIndex == lChapterNr);
            if (lChapter == null) return null;

            IProxerResult<IEnumerable<Manga.Chapter.Page>> lPagesResult = await lChapter.Pages;
            if (!lPagesResult.Success || (lPagesResult.Result == null)) return null;

            MangaPageEnumerable lPageEnumerable = new MangaPageEnumerable(lPagesResult.Result);
            MangaReaderViewModel lMangaReaderViewModel = new MangaReaderViewModel(lChapter, lPageEnumerable, lIsSlide);

            try
            {
                //Try loading the first page to check availability
                await Task.Factory.StartNew(() => lPageEnumerable.Take(1)).ConfigureAwait(true);
            }
            catch (WebException)
            {
                return null;
            }

            IProxerResult<string> lMangaNameResult = await lManga.Name;
            IProxerResult<string> lChapterTitleResult = await lChapter.Title;
            if (!lMangaNameResult.Success || !lChapterTitleResult.Success) return null;

            return lMangaReaderViewModel;
        }

        public async Task LoadPages()
        {
            TaskScheduler lMainContext = TaskScheduler.FromCurrentSynchronizationContext();
            this.IsLoadingPages = true;
            await (await Task.Factory.StartNew(async () =>
            {
                try
                {
                    foreach (IBitmap bitmapSource in this.PageEnumerable)
                        await Task.Factory.StartNew(() => this.PageImages.Add(bitmapSource.ToNative()),
                                CancellationToken.None, TaskCreationOptions.None, lMainContext)
                            .ConfigureAwait(true);
                }
                catch
                {
                    MessageQueue.AddMessage(ResourceUtility.GetString("LoadingErrorMsg"));
                    await Task.Factory.StartNew(NavigationHelper.NavigateBack, CancellationToken.None,
                        TaskCreationOptions.None, lMainContext).ConfigureAwait(true);
                }
            }).ConfigureAwait(true)).ConfigureAwait(true);
            this.IsLoadingPages = false;
        }

        public void OnNavigateBack()
        {
            using (this.PageImages.SuppressChangeNotifications())
            {
                this.PageImages.RemoveRange(0, this.PageImages.Count);
            }
        }

        public void OnViewTapped()
        {
            this.IsContainerVisible = !this.IsContainerVisible;
            if (this.IsContainerVisible) this._containerFadeTimer.Start();
            else this._containerFadeTimer.Stop();
        }

        #endregion
    }
}