using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MMS.UI.BindingConverters
{
    class LastSeenBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime lastseen = (DateTime)value;
            return $"Last Seen {lastseen:MMM dd | h:mm tt}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class StatusTextBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isconfig = (bool)values[0];
            bool isonline = (bool)values[1];
            if (isconfig)
            {
                if (isonline) return "Online";
                else return "Offline";
            }
            else return "Not Configured";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class StatusLedBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stat = (string)value;
            if (stat == "Online") return Brushes.Green;
            if (stat == "Offline") return Brushes.Red;
            if (stat == "Not Configured") return Brushes.Gray;
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ConfigButtonTextBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isconfig = (bool)value;
            if (isconfig) return "Send";
            else return "Register";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
