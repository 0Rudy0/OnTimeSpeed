using HrNetMobile.Models.Vacation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using hrnet = HrNetMobile.Data;
using hrnetModel = HrNetMobile.Models;
using OnTimeSpeed.Utils;
using System.Web.Caching;

namespace OnTimeSpeed.Code
{
    public static class Holidays
    {
        private static hrnet.DAL.VacationAPI _vacationAPI = new hrnet.DAL.VacationAPI();
        public static Dictionary<DateTime, string> GetHolidays(int forYear)
        {
            string cacheKey = "holidaysTxtFile";
            var _holidayList = (Dictionary<DateTime, string>)HttpRuntime.Cache.Get(cacheKey);
            if (_holidayList == null)
            {
                _holidayList = new Dictionary<DateTime, string>();

                var holidaysByYear = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HolidayRange>>(
                System.IO.File.ReadAllText(
                    AppDomain.CurrentDomain.BaseDirectory + "/config/holidays.json", System.Text.Encoding.UTF8));

                holidaysByYear.ForEach(h =>
                {
                    h.yearFromDate = h.yearFrom.ToDate();
                    h.yearToDate = h.yearTo.ToDate();
                });

                var holidaysForNow = holidaysByYear.Where(h => (h.yearFromDate == null || h.yearFromDate.Value.Year >= forYear) &&
                    h.yearToDate == null || h.yearToDate.Value.AddYears(1).AddDays(-1).Year <= forYear);

                holidaysForNow.Select(h => h.holidays).ToList().ForEach(h =>
                {
                    h.ForEach(hh =>
                    {
                        var holidayStr = "";
                        try
                        {
                            holidayStr = $"{hh.date}{forYear}";

                            _holidayList.Add(DateTime.ParseExact(holidayStr, $"dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                hh.name);
                        }
                        catch (Exception ex)
                        {
                            LogUtils.LogException(ex, null, null, "date string: " + holidayStr);
                        }
                    });
                });

                var easter = EasterDate(forYear);
                _holidayList.Add(easter, "Uskrs"); //Uskrs
                _holidayList.Add(easter.AddDays(1), "Uskršnji ponedjeljak"); //Uskršnji ponedjeljak
                _holidayList.Add(Tijelovo(easter), "Tijelovo"); //Tijelovo

                HttpRuntime.Cache.Insert(cacheKey,
                    _holidayList,
                    null,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(1440), //1 dan
                    CacheItemPriority.Normal,
                    null);
            }



            return _holidayList;
        }

        /// <summary>
        /// Gaussov algoritam
        /// https://bs.wikipedia.org/wiki/Ra%C4%8Dunanje_datuma_Uskrsa
        /// </summary>
        /// <param name="forYear"></param>
        /// <returns></returns>
        public static DateTime EasterDate(int forYear)
        {
            var a = forYear % 19;
            var b = forYear % 4;
            var c = forYear % 7;

            var x = 24;
            var y = 5;

            var d = (a * 19 + x) % 30;
            var e = ((2 * b) + (4 * c) + (6 * d) + y) % 7;

            var option1 = 22 + d + e;
            DateTime easterDate;
            if (option1 > 31)
            {
                option1 = d + e - 9;
                easterDate = new DateTime(forYear, 4, option1);
            }
            else
            {
                easterDate = new DateTime(forYear, 3, option1);
            }

            return easterDate;
        }

        /// <summary>
        /// deveti četvrtak nakon Uskrsa
        /// </summary>
        /// <param name="easterDate"></param>
        /// <returns></returns>
        private static DateTime Tijelovo(DateTime easterDate)
        {
            var tijelovoDate = easterDate;
            for (var i = 0; i < 8; i++)
            {
                tijelovoDate = tijelovoDate.AddDays(7);
            }
            tijelovoDate = tijelovoDate.AddDays(4);

            return tijelovoDate;
        }
    }

    class HolidayRange
    {
        public string yearFrom { get; set; }
        public string yearTo { get; set; }
        public DateTime? yearFromDate { get; set; }
        public DateTime? yearToDate { get; set; }

        public List<HolidayManually> holidays { get; set; }
    }

    class HolidayManually
    {
        public string name { get; set; }
        public string date { get; set; }
    }
}