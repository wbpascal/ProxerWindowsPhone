using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Proxer.ViewModels.Media;

namespace Proxer.UserControls
{
    public sealed partial class MangaReaderContainer : UserControl
    {
        public static readonly DependencyProperty ReaderContentProperty = DependencyProperty.Register(
            nameof(ReaderContent), typeof(object), typeof(MangaReaderContainer), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(MangaReaderViewModel), typeof(MangaReaderContainer),
            new PropertyMetadata(default(MangaReaderViewModel)));

        public MangaReaderContainer()
        {
            this.DataContext = this;
            this.InitializeComponent();
        }

        #region Properties

        public object ReaderContent
        {
            get { return this.GetValue(ReaderContentProperty); }
            set { this.SetValue(ReaderContentProperty, value); }
        }

        public MangaReaderViewModel ViewModel
        {
            get { return (MangaReaderViewModel) this.GetValue(ViewModelProperty); }
            set { this.SetValue(ViewModelProperty, value); }
        }

        #endregion
    }
}