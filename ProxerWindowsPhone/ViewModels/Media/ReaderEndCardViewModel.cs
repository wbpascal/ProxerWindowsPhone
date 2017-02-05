using ReactiveUI;

namespace Proxer.ViewModels.Media
{
    public class ReaderEndCardViewModel : ReactiveObject
    {
        public ReaderEndCardViewModel(bool isLastChapter, ReactiveCommand navigateBackCommand,
            ReactiveCommand navigateNextChapterCommand)
        {
            this.IsLastChapter = isLastChapter;
            this.NavigateBackCommand = navigateBackCommand;
            this.NavigateNextChapterCommand = navigateNextChapterCommand;
        }

        #region Properties

        public bool IsLastChapter { get; set; }

        public ReactiveCommand NavigateBackCommand { get; set; }

        public ReactiveCommand NavigateNextChapterCommand { get; set; }

        #endregion
    }
}