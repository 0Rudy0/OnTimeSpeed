using OnTimeSpeed.Models;
using OnTimeSpeed.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OnTimeSpeed.Code
{
    public enum GroupBy
    {
        Month = 1,
        Week = 2, 
        Day = 3
    }
    public static class PrepareData
    {
        public static HighChartsData PrepareWorkLogChartData(
            List<WorkLog> workLogs, 
            GroupBy groupBy, 
            DateTime fromDate, DateTime? toDate)
        {
            var chartData = new HighChartsData();
            var groupedData = new Dictionary<string, float>();
            var plannedAmount = new Dictionary<string, float>();
            toDate = toDate ?? DateTime.Now;

            for (var i = fromDate; i <= toDate; i = i.AddDays(1))
            {
                if (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                var forMonth = i.Month;
                var forWeek = GetWeekNumber(i);
                var forYear = i.ToString("yyyy");

                var key = "";
                switch (groupBy)
                {
                    case GroupBy.Day:
                        key = i.ToString("dd.MM.yyyy.");
                        break;
                    case GroupBy.Week:
                        key = $"{forWeek} tj @ {forYear}.";
                        break;
                    case GroupBy.Month:
                        key = i.ToString("MMMM yyyy.");// $"{forMonth}/{forYear}";
                        break;
                }
                var valToAdd = 8;
                if (i > DateTime.Now)
                    valToAdd = 0;

                if (!groupedData.ContainsKey(key))
                {
                    groupedData.Add(key, 0);
                    plannedAmount.Add(key, valToAdd);
                }
                else
                {
                    plannedAmount[key] += valToAdd;
                }
            }


            foreach (var log in workLogs)
            {
                var forDate = log.date_time;

                var forMonth = forDate.Month;
                var forWeek = GetWeekNumber(forDate);
                var forYear = forDate.ToString("yyyy");

                var key = "";
                switch (groupBy)
                {
                    case GroupBy.Day:
                        key = forDate.ToString("dd.MM.yyyy.");
                        break;
                    case GroupBy.Week:
                        key = $"{forWeek} tj @ {forYear}.";
                        break;
                    case GroupBy.Month:
                        key = forDate.ToString("MMMM yyyy.");// $"{forMonth}/{forYear}";
                        break;
                }

                if (!groupedData.ContainsKey(key))
                    groupedData.Add(key, log.work_done.duration_minutes / 60);
                else
                    groupedData[key] += log.work_done.duration_minutes / 60;
            }

            var categories = plannedAmount.Select(g => g.Key).ToList();
            categories.AddRange(groupedData.Select(g => g.Key).ToList());

            chartData.Categories_id = categories.Distinct().ToList();
            chartData.Categories_names = categories.Distinct().ToList();
            chartData.Series.Add(new HighChartsData.HighChartsSerie
            {
                SerieId = "Planirano",
                SerieName = "Planirano",
                ValuesArray = new List<float>()
            });
            chartData.Series.Add(new HighChartsData.HighChartsSerie
            {
                SerieId = "Ostvareno",
                SerieName = "Ostvareno",
                ValuesArray = new List<float>()
            });

            foreach (var key in chartData.Categories_names)
            {
                if (plannedAmount.ContainsKey(key))
                    chartData.Series.FirstOrDefault(s => s.SerieId == "Planirano").ValuesArray.Add(plannedAmount[key]);
                else
                    chartData.Series.FirstOrDefault(s => s.SerieId == "Planirano").ValuesArray.Add(0);

                if (groupedData.ContainsKey(key))
                    chartData.Series.FirstOrDefault(s => s.SerieId == "Ostvareno").ValuesArray.Add(groupedData[key]);
                else
                    chartData.Series.FirstOrDefault(s => s.SerieId == "Ostvareno").ValuesArray.Add(0);
            }

            return chartData;
        }

        public static int GetWeekNumber(DateTime forDate)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(forDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }

        public static Tuple<DateTime, DateTime?> GetDateRange(string forDate, GroupBy groupping)
        {
            var dateFrom = DateTime.Now;
            DateTime? dateTo = null;
            var prevMonthStart = dateFrom.AddMonths(-1);
            prevMonthStart = new DateTime(prevMonthStart.Year, prevMonthStart.Month, 1);
            dateFrom = prevMonthStart;

            if (String.IsNullOrEmpty(forDate))
            {
                return new Tuple<DateTime, DateTime?>(dateFrom, dateTo);
            }
            else
            {
                string[] parts = new string[2];

                switch (groupping)
                {
                    case GroupBy.Month:
                        dateFrom = DateTime.ParseExact(forDate, "MMMM yyyy.", CultureInfo.CurrentCulture);// forDate.Split('/');
                        //dateFrom = new DateTime(int.Parse(parts[1]), int.Parse(parts[0]), 1);
                        dateTo = dateFrom.AddMonths(1).AddDays(-1); //zadnji dan u mjesecu
                        break;
                    case GroupBy.Week:
                        parts = forDate.Split('@');
                        var week = int.Parse(parts[0].Replace(" tj ", "").Trim());
                        var year = int.Parse(parts[1].Replace(".", "").Trim());
                        var fromSet = false;

                        for (var i = prevMonthStart; i < DateTime.Now.AddMonths(1); i = i.AddDays(1))
                        {
                            var forWeek = GetWeekNumber(i);
                            if (forWeek == week)
                            {
                                dateTo = i;
                                if (!fromSet)
                                {
                                    dateFrom = i;
                                    fromSet = true;
                                }
                            }
                            else if (forWeek > week)
                                break;
                        }
                        break;
                }                

                return new Tuple<DateTime, DateTime?>(dateFrom, dateTo);
            }

        }

        public static DateTime? DateFromName(string date, string template)
        {
            if (template.Length != date.Length)
                return null;

            try
            {
                var monthStr = "";
                var yearStr = "";
                for (var i = 0; i < template.Length; i++)
                {
                    if (template[i] == 'M')
                        monthStr += date[i];
                    else if (template[i] == 'y')
                        yearStr += date[i];

                    //if (template[i] == '?')
                    //{
                    //    date = date.Remove(i, 1);
                    //    date = date.Insert(i, "?");
                    //}
                }

                var dateFrom = DateTime.ParseExact($"{monthStr}/{yearStr}", "MM/yyyy", CultureInfo.InvariantCulture);

                return dateFrom;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static object CreateLunchWorkLogObj(int userId, int itemId, DateTime forDate)
        {
            return CreateWorkLogObject(userId, 0.5f, AppSettings.GetInt("rucakWorkLogType"), itemId, "tasks", forDate);
        }

        public static object CreateWorkLogObject(int userId, float durationHrs, int logType, int itemId, string itemType, DateTime forDate)
        {
            return new
            {
                user = new
                {
                    id = userId
                },
                work_done = new
                {
                    duration = durationHrs,
                    time_unit = new
                    {
                        id = 2
                    }
                },
                work_log_type = new
                {
                    id = logType
                },
                item = new
                {
                    id = itemId,
                    item_type = itemType
                },
                date_time = forDate.ToString("yyyy-MM-dd")
            };
        }

        public static WorkItem GetLunchTaskForDate(List<WorkItem> lunchItems, DateTime forDate)
        {            
            foreach (var lunchItem in lunchItems)
            {
                var fromDate = DateFromName(lunchItem.Name, AppSettings.Get("rucakDateFromTemplate"));
                var toDate = DateFromName(lunchItem.Name, AppSettings.Get("rucakDateToTemplate"));

                if (fromDate == null || toDate == null)
                    continue;
                else
                {
                    if (fromDate <= forDate && forDate <= toDate)
                        return lunchItem;
                }
            }

            return null;
        }

        public static bool DoesLogAlreadyExist(List<WorkLog> logs, WorkItem newItem, DateTime onDate)
        {
            var exist = false;
            foreach (var log in logs)
            {
                if (log.date_time.Date == onDate.Date && log.item.id == newItem.Id)
                {
                    exist = true;
                    break;
                }
            }

            return exist;
        }
    }
}