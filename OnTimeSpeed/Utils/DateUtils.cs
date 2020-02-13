using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace OnTimeSpeed.Utils
{
    public static class DateUtils
    {
        public static List<DateTime> Holidays = new List<DateTime>();

        public static string ToStringCustom(this DateTime date)
        {
            var dateFormat = "yyyy-MM-dd";
            return date.ToString(dateFormat, CultureInfo.InvariantCulture);
        }

        public static DateTime ToFirstOfMonth(this DateTime date)
        {
            var returnDate = new DateTime(date.Year, date.Month, 1);
            return returnDate;
        }

        public static bool IsHoliday(this DateTime date)
        {
            return Holidays.Contains(date.Date);
        }

        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday;
        }
    }
}