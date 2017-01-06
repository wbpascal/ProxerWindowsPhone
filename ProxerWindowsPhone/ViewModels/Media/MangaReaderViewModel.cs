using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Azuria.ErrorHandling;
using Azuria.Exceptions;
using Azuria.Media;
using Azuria.Media.Properties;
using Proxer.Utility;
using ReactiveUI;

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

        public MangaReaderViewModel(Manga.Chapter chapter, IEnumerable<Uri> pageUris, bool isSlide)
        {
            this.Chapter = chapter;
            this.IsSlide = isSlide;
            this.PageImages.AddRange(pageUris);

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

        public bool IsSlide { get; }

        public ReactiveCommand NavigateBackCommand { get; }

        public ReactiveList<Uri> PageImages { get; } = new ReactiveList<Uri>();

        #endregion

        #region Methods

        private void ContainerFadeTimerOnTick(object sender, object o)
        {
            this.IsContainerVisible = false;
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

            IProxerResult<string> lMangaNameResult = await lManga.Name;
            IProxerResult<string> lChapterTitleResult = await lChapter.Title;
            if (!lMangaNameResult.Success || !lChapterTitleResult.Success) return null;

            return new MangaReaderViewModel(lChapter, lPagesResult.Result.Select(page => page.Image), lIsSlide);
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