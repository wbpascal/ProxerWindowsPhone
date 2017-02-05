using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Azuria.ErrorHandling;
using Azuria.Exceptions;
using Azuria.Media;
using Azuria.Media.Properties;
using Proxer.UserControls.MangaReader;
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

        public MangaReaderViewModel(Manga.Chapter chapter, IEnumerable<Uri> pageUris, bool isSlide, bool isLastChapter)
        {
            this.Chapter = chapter;
            this.IsSlide = isSlide;

            this.NavigateBackCommand = ReactiveCommand.Create(NavigationHelper.NavigateBack);

            Uri[] lPageUris = pageUris as Uri[] ?? pageUris.ToArray();
            this.PageImages.AddRange(lPageUris.Select(uri => new ReaderPage {UriSource = uri.AbsoluteUri}));
            this.PageImages.Add(new ReaderEndCard
            {
                UriSource = lPageUris.LastOrDefault()?.AbsoluteUri,
                ViewModel = this.CreateEndCardViewModel(chapter, isLastChapter)
            });

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

        public ReactiveList<IReaderPage> PageImages { get; } = new ReactiveList<IReaderPage>();

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

            return new MangaReaderViewModel(lChapter, lPagesResult.Result.Select(page => page.Image), lIsSlide, false);
        }

        private ReaderEndCardViewModel CreateEndCardViewModel(Manga.Chapter chapter, bool isLastChapter)
        {
            Uri lNextChapterUri = new Uri($"https://proxer.me/chapter/{chapter.ParentObject.Id}" +
                                          $"/{chapter.ContentIndex + 1}" +
                                          $"/{LanguageUtility.GetChapterString(chapter.GeneralLanguage)}");
            ReactiveCommand<Unit, Unit> lNavigateNextChapterCommand = ReactiveCommand.Create(() =>
            {
                NavigationHelper.NavigateBack();
                NavigationHelper.NavigateToUrl(lNextChapterUri);
            });
            return new ReaderEndCardViewModel(isLastChapter, this.NavigateBackCommand, lNavigateNextChapterCommand);
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