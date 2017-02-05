using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Proxer.Utility.Converter
{
    public class BooleanToVisibilityCollapsedConverter : IValueConverter
    {
        #region Properties

        public bool Invert { get; set; }

        #endregion

        #region Methods

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((bool) value && !this.Invert) || (!(bool) value && this.Invert)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }

        #endregion
    }
}