using System;
using Windows.Phone.UI.Input;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Proxer.Views.Media
{
    public sealed partial class MediaPlayerView
    {
        private DisplayRequest _appDisplayRequest;

        public MediaPlayerView()
        {
            HardwareButtons.BackPressed += this.HardwareButtonsOnBackPressed;
            this.InitializeComponent();
        }

        #region

        private void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs backPressedEventArgs)
        {
            this.MediaPlayer.Stop();
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