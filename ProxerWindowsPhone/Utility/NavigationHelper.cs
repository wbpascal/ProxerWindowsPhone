using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Proxer.Views;

namespace Proxer.Utility
{
    public static class NavigationHelper
    {
        #region Methods

        public static void NavigateBack()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if ((rootFrame != null) && rootFrame.CanGoBack) rootFrame.GoBack();
            NavigateTo(typeof(MainView), null);
        }

        public static bool NavigateTo(Type pageType, object parameter)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            return rootFrame?.Navigate(pageType, parameter) ?? false;
        }

        #endregion
    }
}