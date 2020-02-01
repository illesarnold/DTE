using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DTE.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
   public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value is bool)
            {
                flag = (bool)value;
            }
            else if (value is bool?)
            {
                bool? nullable = (bool?)value;
                flag = nullable.HasValue ? nullable.Value : false;
            }
            return (flag ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility flag = Visibility.Visible;
            if (value is Visibility.Visible)
            {
                flag = (Visibility)value;
            }
            else if (value is Visibility.Collapsed)
            {
                Visibility nullable = (Visibility)value;
            }

            return (flag == Visibility.Visible ? true : false);
        }
    }
}
