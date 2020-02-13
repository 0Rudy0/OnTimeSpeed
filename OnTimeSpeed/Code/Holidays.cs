using HrNetMobile.Models.Vacation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using hrnet = HrNetMobile.Data;
using hrnetModel = HrNetMobile.Models;

namespace OnTimeSpeed.Code
{
    public static class Holidays
    {
        private static hrnet.DAL.VacationAPI _vacationAPI = new hrnet.DAL.VacationAPI();
        public static List<DateTime> GetHolidays(int forYear, hrnetModel.User hrproUser)
        {
            var _holidayList = new List<DateTime>();
            _holidayList.Add(new DateTime(forYear, 1, 1)); //Nova godina
            _holidayList.Add(new DateTime(forYear, 1, 6)); //Sveta tri kralja
            _holidayList.Add(new DateTime(forYear, 5, 1)); //Praznik rada
            _holidayList.Add(new DateTime(forYear, 6, 22)); //Dan antifašističke borbe
            _holidayList.Add(new DateTime(forYear, 8, 5)); //Dan domovinske zahvalnosti
            _holidayList.Add(new DateTime(forYear, 8, 15)); //Velika gospa
            _holidayList.Add(new DateTime(forYear, 11, 1)); //Dan svih svetih
            _holidayList.Add(new DateTime(forYear, 12, 25)); //Božić
            _holidayList.Add(new DateTime(forYear, 12, 26)); //Štefanje

            var easter = EasterDate(forYear);
            _holidayList.Add(easter); //Uskrs
            _holidayList.Add(easter.AddDays(1)); //Uskršnji ponedjeljak
            _holidayList.Add(Tijelovo(easter)); //Tijelovo

            if (forYear >= 2020)
            {
                _holidayList.Add(new DateTime(forYear, 5, 30)); //Dan državnosti
                _holidayList.Add(new DateTime(forYear, 11, 18)); //Dan sjećanja na žrtve Domovinskog rata
            }

            else if (forYear <= 2019)
            {
                _holidayList.Add(new DateTime(forYear, 6, 25)); //Dan državnosti
                _holidayList.Add(new DateTime(forYear, 10, 8)); //Dan nezavisnosti
            }

            return _holidayList.OrderBy(h => h).ToList();
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
        private static DateTime Tijelovo (DateTime easterDate)
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
}