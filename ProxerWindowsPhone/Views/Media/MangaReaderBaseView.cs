using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Proxer.Utility;
using Proxer.ViewModels.Media;
using ReactiveUI;

namespace Proxer.Views.Media
{
    public abstract class MangaReaderBaseView : Page
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(MangaReaderViewModel), typeof(MangaReaderBaseView),
            new PropertyMetadata(default(MangaReaderViewModel)));

        protected MangaReaderBaseView()
        {
            this.WhenAnyValue(view => view.ViewModel).Subscribe(this.OnNewViewModel);
        }

        #region Properties

        public MangaReaderViewModel ViewModel
        {
            get { return (MangaReaderViewModel) this.GetValue(ViewModelProperty); }
            set { this.SetValue(ViewModelProperty, value); }
        }

        #endregion

        #region Methods

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.ViewModel = e.Parameter as MangaReaderViewModel;
            if (this.ViewModel != null) return;
            MessageQueue.AddMessage("Fehler beim Laden des Manga! Bitte versuche es später erneut!");
            NavigationHelper.NavigateBack();
        }

        protected virtual void OnNewViewModel(MangaReaderViewModel viewModel)
        {
            viewModel?.LoadPages().ConfigureAwait(true);
        }

        /// <inheritdoc />
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            this.ViewModel?.OnViewTapped();
        }

        #endregion
    }
}