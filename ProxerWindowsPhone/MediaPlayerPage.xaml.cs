using System;
using Windows.Phone.UI.Input;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Proxer
{
    public sealed partial class MediaPlayerPage
    {
        private DisplayRequest _appDisplayRequest;

        public MediaPlayerPage()
        {
            HardwareButtons.BackPressed += this.HardwareButtonsOnBackPressed;
            this.InitializeComponent();
        }

        #region

        private void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs backPressedEventArgs)
        {
            if (backPressedEventArgs.Handled ||
                ((Window.Current.Content as Frame)?.SourcePageType != typeof(MediaPlayerPage))) return;

            backPressedEventArgs.Handled = true;
            this.MediaPlayer.Stop();

            Frame rootFrame = (Frame) Window.Current.Content;
            if (rootFrame.CanGoBack) rootFrame.GoBack();
            else rootFrame.Navigate(typeof(MainPage), null);
        }

        private void MediaPlayer_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (this.MediaPlayer.IsAudioOnly) return;

            if (this.MediaPlayer.CurrentState == MediaElementState.Playing)
            {
                if (this._appDisplayRequest != null) return;
                this._appDisplayRequest = new DisplayRequest();
                this._appDisplayRequest.RequestActive();
            }
            else
            {
                if (this._appDisplayRequest == null) return;
                this._appDisplayRequest.RequestRelease();
                this._appDisplayRequest = null;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!(e.Parameter is Uri)) return;
            this.MediaPlayer.Source = (Uri) e.Parameter;
            this.MediaPlayer.Play();
        }

        #endregion
    }
}