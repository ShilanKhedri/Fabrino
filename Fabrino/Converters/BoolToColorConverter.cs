using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Fabrino.Converters
{
    /// <summary>
    /// Converts boolean values to color brushes for UI elements
    /// Used primarily for admin status indication in the interface
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a color brush
        /// True returns a salmon color (admin), False returns white (regular user)
        /// </summary>
        /// <returns>SolidColorBrush based on the boolean input</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAdmin)
            {
                return isAdmin ? new SolidColorBrush(Color.FromRgb(244, 134, 114)) : new SolidColorBrush(Colors.White);
            }
            return new SolidColorBrush(Colors.White);
        }

        /// <summary>
        /// Convert back operation is not supported in this converter
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}