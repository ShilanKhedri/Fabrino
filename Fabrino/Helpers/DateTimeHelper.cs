using System;
using System.Globalization;

namespace Fabrino.Helpers
{
    public static class DateTimeHelper
    {
        public static string ToPersianDateTime(DateTime? date)
        {
            if (!date.HasValue) return "تاریخ نامعتبر";
            
            try
            {
                PersianCalendar pc = new PersianCalendar();
                return $"{pc.GetYear(date.Value):0000}/{pc.GetMonth(date.Value):00}/{pc.GetDayOfMonth(date.Value):00} - " +
                       $"{date.Value.Hour:00}:{date.Value.Minute:00}";
            }
            catch
            {
                return "تاریخ نامعتبر";
            }
        }
    }
} 