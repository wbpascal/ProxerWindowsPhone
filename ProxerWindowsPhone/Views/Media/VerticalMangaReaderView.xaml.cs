using Windows.UI.Xaml.Controls;

namespace Proxer.Views.Media
{
    public sealed partial class VerticalMangaReaderView : MangaReaderBaseView
    {
        public VerticalMangaReaderView()
        {
            this.DataContext = this;
            this.InitializeComponent();
        }

        #region Methods

        private void PageScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (this.ViewModel == null) return;
            this.ViewModel.CurrentPageIndex =
                ((this.PagePresenter.ItemsPanelRoot as ItemsStackPanel)?.FirstVisibleIndex ?? 0) + 1;
        }

        #endregion
    }
}