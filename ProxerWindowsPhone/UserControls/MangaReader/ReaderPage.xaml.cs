using Windows.UI.Xaml;

namespace Proxer.UserControls.MangaReader
{
    public sealed partial class ReaderPage : IReaderPage
    {
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register(
            nameof(UriSource), typeof(string), typeof(ReaderPage), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ZoomMaxHeightProperty = DependencyProperty.Register(
            nameof(ZoomMaxHeight), typeof(double), typeof(ReaderPage), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty ZoomMaxWidthProperty = DependencyProperty.Register(
            nameof(ZoomMaxWidth), typeof(double), typeof(ReaderPage), new PropertyMetadata(default(double)));

        public ReaderPage()
        {
            this.InitializeComponent();

            this.ZoomMaxHeight = Window.Current?.Bounds.Height ?? 0;
            this.ZoomMaxWidth = Window.Current?.Bounds.Width ?? 0;
        }

        #region Properties

        public string UriSource
        {
            get { return (string) this.GetValue(UriSourceProperty); }
            set { this.SetValue(UriSourceProperty, value); }
        }

        public double ZoomMaxHeight
        {
            get { return (double) this.GetValue(ZoomMaxHeightProperty); }
            set { this.SetValue(ZoomMaxHeightProperty, value); }
        }

        public double ZoomMaxWidth
        {
            get { return (double) this.GetValue(ZoomMaxWidthProperty); }
            set { this.SetValue(ZoomMaxWidthProperty, value); }
        }

        #endregion

        #region Methods

        public void ResetZoom()
        {
            this.LayoutRoot.ChangeView(null, null, 1.0f);
        }

        #endregion
    }
}