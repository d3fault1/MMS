using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MMS.UI.BindingConverters
{
    class BooleanToVisibilityInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool input = (bool)value;
            if (input) return Visibility.Collapsed;
            else return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility input = (Visibility)value;
            if (input == Visibility.Collapsed) return true;
            else if (input == Visibility.Visible) return false;
            else return false;
        }
    }
}
