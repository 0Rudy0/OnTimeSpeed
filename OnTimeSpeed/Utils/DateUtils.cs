using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace OnTimeSpeed.Utils
{
    public static class DateUtils
    {
        public static string ToStringCustom(this DateTime date)
        {
            var dateFormat = "yyyy-MM-dd";
            return date.ToString(dateFormat, CultureInfo.InvariantCulture);
        }
    }
}