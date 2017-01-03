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
            Frame lRootFrame = Window.Current.Content as Frame;
            if ((lRootFrame != null) && lRootFrame.CanGoBack) lRootFrame.GoBack();
            NavigateTo(typeof(MainView), null);
        }

        public static bool NavigateTo(Type pageType, object parameter)
        {
            Frame lRootFrame = Window.Current.Content as Frame;
            return lRootFrame?.Navigate(pageType, parameter) ?? false;
        }

        public static void NavigateToUrl(Uri url)
        {
            Frame lRootFrame = Window.Current.Content as Frame;
            MainView lMainView = lRootFrame?.Content as MainView;
            lMainView?.NavigateToUrl(url);
        }

        #endregion
    }
}