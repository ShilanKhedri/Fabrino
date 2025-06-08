using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Fabrino.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAdmin)
            {
                return isAdmin ? new SolidColorBrush(Color.FromRgb(244, 134, 114)) : new SolidColorBrush(Colors.White);
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}