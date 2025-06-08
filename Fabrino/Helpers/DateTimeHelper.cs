using System;
using System.Globalization;

namespace Fabrino.Helpers
{
    /// <summary>
    /// Provides date and time conversion utilities, specifically for Persian calendar
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Converts a nullable DateTime to Persian calendar format string
        /// </summary>
        /// <param name="date">The DateTime to convert</param>
        /// <returns>Formatted string in Persian calendar (YYYY/MM/DD - HH:mm) or "Invalid Date" if conversion fails</returns>
        public static string ToPersianDateTime(DateTime? date)
        {
            if (!date.HasValue) return "Invalid Date";
            
            try
            {
                PersianCalendar pc = new PersianCalendar();
                return $"{pc.GetYear(date.Value):0000}/{pc.GetMonth(date.Value):00}/{pc.GetDayOfMonth(date.Value):00} - " +
                       $"{date.Value.Hour:00}:{date.Value.Minute:00}";
            }
            catch
            {
                return "Invalid Date";
            }
        }
    }
} 