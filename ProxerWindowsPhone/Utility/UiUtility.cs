using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Proxer.Utility
{
    public static class UiUtility
    {
        #region Methods

        public static T FindDescendant<T>(this DependencyObject obj) where T : DependencyObject
        {
            T lDescendant = obj as T;
            if (lDescendant != null)
                return lDescendant;

            int lChildrenCount = VisualTreeHelper.GetChildrenCount(obj);
            if (lChildrenCount == 0)
                return null;

            for (int i = 0; i < lChildrenCount; i++)
            {
                T lChild = FindDescendant<T>(VisualTreeHelper.GetChild(obj, i));
                if (lChild != null)
                    return lChild;
            }

            return null;
        }

        #endregion
    }
}