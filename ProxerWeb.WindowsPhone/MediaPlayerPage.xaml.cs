using System;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Proxer
{
    public sealed partial class MediaPlayerPage : Page
    {
        public MediaPlayerPage()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += this.HardwareButtonsOnBackPressed;
        }

        #region

        private void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs backPressedEventArgs)
        {
            if (backPressedEventArgs.Handled ||
                (Window.Current.Content as Frame)?.SourcePageType != typeof(MediaPlayerPage)) return;

            backPressedEventArgs.Handled = true;
            Frame rootFrame = (Frame) Window.Current.Content;
            if (rootFrame.CanGoBack) rootFrame.GoBack();
            else rootFrame.Navigate(typeof(MainPage), null);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!(e.Parameter is Uri)) return;
            this.MediaPlayerControl.Source = (Uri) e.Parameter;
            this.MediaPlayerControl.Play();
        }

        #endregion
    }
}