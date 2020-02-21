using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace OnTimeSpeed.Utils
{
    public static class DateUtils
    {
        public static Dictionary<DateTime, string> Holidays = new Dictionary<DateTime, string>();

        public static string ToStringCustom(this DateTime date)
        {
            var dateFormat = "yyyy-MM-dd";
            return date.ToString(dateFormat, CultureInfo.InvariantCulture);
        }

        public static DateTime? ToDate(this string dateStr)
        {
            try
            {
                var dateFormat = "dd.MM.yyyy";
                return DateTime.ParseExact(dateStr, dateFormat, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
                return null;
            }

        }

        public static DateTime ToFirstOfMonth(this DateTime date)
        {
            var returnDate = new DateTime(date.Year, date.Month, 1);
            return returnDate;
        }
        public static DateTime ToLastOfMonth(this DateTime date)
        {
            var returnDate = date.ToFirstOfMonth().AddMonths(1).AddDays(-1);
            return returnDate;
        }

        public static bool IsHoliday(this DateTime date)
        {
            return Holidays.ContainsKey(date.Date);
        }

        public static string HolidayName(this DateTime date)
        {
            return Holidays.ContainsKey(date.Date) ? Holidays[date.Date] : String.Empty;
        }

        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday;
        }
    }
}