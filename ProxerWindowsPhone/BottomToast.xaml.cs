using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Proxer
{
    public sealed partial class BottomToast : UserControl
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            nameof(Message), typeof(string), typeof(BottomToast), new PropertyMetadata(default(string)));

        public BottomToast()
        {
            this.DataContext = this;
            this.InitializeComponent();
        }

        #region Properties

        public string Message
        {
            get { return (string) this.GetValue(MessageProperty); }
            set { this.SetValue(MessageProperty, value); }
        }

        #endregion

        #region

        public void Activate()
        {
            this.ShowAnimation.Begin();
        }

        #endregion
    }
}