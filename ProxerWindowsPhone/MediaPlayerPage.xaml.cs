using System;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Microsoft.PlayerFramework;

namespace Proxer
{
    public sealed partial class MediaPlayerPage : Page
    {
        private readonly DoublePressHandler _backDoublePressHandler;

        public MediaPlayerPage()
        {
            this._backDoublePressHandler = new DoublePressHandler(TimeSpan.FromSeconds(2.5));
            this._backDoublePressHandler.PressedOnce += this.BackDoublePressHandler_PressedOnce;
            this._backDoublePressHandler.PressedTwice += this.BackDoublePressHandler_PressedTwice;
            HardwareButtons.BackPressed += this.HardwareButtonsOnBackPressed;

            this.InitializeComponent();
        }

        #region

        private void BackDoublePressHandler_PressedOnce(object sender, EventArgs e)
        {
            this.BottomToast.Activate();
        }

        private void BackDoublePressHandler_PressedTwice(object sender, EventArgs e)
        {
            Frame rootFrame = (Frame) Window.Current.Content;
            if (rootFrame.CanGoBack) rootFrame.GoBack();
            else rootFrame.Navigate(typeof(MainPage), null);
        }

        private void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs backPressedEventArgs)
        {
            if (backPressedEventArgs.Handled ||
                (Window.Current.Content as Frame)?.SourcePageType != typeof(MediaPlayerPage)) return;

            backPressedEventArgs.Handled = true;
            if (this.MediaPlayerControl.IsFullScreen) this.MediaPlayerControl.IsFullScreen = false;
            this._backDoublePressHandler.Pressed();
        }

        private void MediaPlayerControl_IsFullScreenChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            (sender as MediaPlayer).IsFullWindow = e.NewValue;
        }

        private void MediaPlayerControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            MediaPlayer lPlayer = sender as MediaPlayer;
            if (lPlayer == null) return;
            if (lPlayer.IsFullWindow) lPlayer.IsFullScreen = false;
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