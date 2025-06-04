using System;
using System.Globalization;
using System.Windows.Data;

namespace Fabrino.Helpers
{
    public class DateTimeToPersianDateConverter : IValueConverter
    {
        private readonly PersianCalendar pc = new PersianCalendar();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
                return $"{pc.GetYear(dt):0000}/{pc.GetMonth(dt):00}/{pc.GetDayOfMonth(dt):00}";
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
