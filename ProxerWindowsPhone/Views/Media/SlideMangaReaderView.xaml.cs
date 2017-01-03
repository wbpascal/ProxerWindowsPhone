using System;
using Windows.UI.Xaml.Controls;
using Proxer.Utility;
using Proxer.ViewModels.Media;
using ReactiveUI;

namespace Proxer.Views.Media
{
    public sealed partial class SlideMangaReaderView : MangaReaderBaseView
    {
        public SlideMangaReaderView()
        {
            this.DataContext = this;
            this.InitializeComponent();
            this.PagePresenter.SelectionChanged += this.OnPagePresenterOnSelectionChanged;
        }

        #region Methods

        /// <inheritdoc />
        protected override void OnNewViewModel(MangaReaderViewModel viewModel)
        {
            base.OnNewViewModel(viewModel);

            if (viewModel == null) return;
            this.PagePresenter.WhenAnyValue(presenter => presenter.SelectedIndex)
                .Subscribe(index => { this.ViewModel.CurrentPageIndex = index + 1; });
        }

        private void OnPagePresenterOnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            this.PagePresenter.ContainerFromIndex(this.PagePresenter.SelectedIndex)?
                .FindDescendant<ScrollViewer>()?
                .ChangeView(null, null, 1.0f);
        }

        #endregion
    }
}