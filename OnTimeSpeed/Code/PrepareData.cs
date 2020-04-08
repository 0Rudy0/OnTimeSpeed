using OnTimeSpeed.Models;
using OnTimeSpeed.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OnTimeSpeed.Utils;
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
            var overAmount = new Dictionary<string, bool>();
            var underAmount = new Dictionary<string, bool>();

            var workLogsOnDate = new Dictionary<string, List<WorkLogFriendly>>();

            toDate = toDate ?? new DateTime(Math.Max(DateTime.Now.ToLastOfMonth().Ticks, workLogs.LastOrDefault().date_time.Date.Ticks));

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
                        key = $"{forWeek}. tj @ {forYear}.";
                        //key = $"{i.}"
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
                        key = $"{forWeek}. tj @ {forYear}.";
                        break;
                    case GroupBy.Month:
                        key = forDate.ToString("MMMM yyyy.");// $"{forMonth}/{forYear}";
                        break;
                }

                if (!groupedData.ContainsKey(key) && (fromDate == null || log.date_time.Date >= fromDate) && (toDate == null || log.date_time.Date <= toDate))
                {
                    groupedData.Add(key, log.work_done.duration_minutes / 60);

                    if (!workLogsOnDate.ContainsKey(key))
                    {
                        workLogsOnDate.Add(key, new List<WorkLogFriendly>());
                    }
                    workLogsOnDate[key].Add(new WorkLogFriendly
                    {
                        LogId = log.id,
                        ItemName = log.item.name,
                        Amount = log.work_done.duration_minutes / 60,
                        Descripton = log.description,
                        WorkType = log.work_log_type.name
                    });
                    workLogsOnDate[key] = workLogsOnDate[key].OrderByDescending(v => v.Amount).ToList();
                }
                else if (groupedData.ContainsKey(key) && (fromDate == null || log.date_time.Date >= fromDate) && (toDate == null || log.date_time.Date <= toDate))
                {
                    groupedData[key] += log.work_done.duration_minutes / 60;

                    if (!workLogsOnDate.ContainsKey(key))
                    {
                        workLogsOnDate.Add(key, new List<WorkLogFriendly>());
                    }
                    workLogsOnDate[key].Add(new WorkLogFriendly
                    {
                        LogId = log.id,
                        ItemName = log.item.name,
                        Amount = log.work_done.duration_minutes / 60,
                        Descripton = log.description,
                        WorkType = log.work_log_type.name
                    });

                    workLogsOnDate[key] = workLogsOnDate[key].OrderByDescending(v => v.Amount).ToList();
                }                
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
            chartData.WorkLogs = workLogsOnDate;

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
            var prevMonthStart = dateFrom.AddMonths(AppSettings.GetInt("monthsBack") * -1);
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
                        var week = int.Parse(parts[0].Replace(". tj ", "").Trim());
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
                            else if (forWeek > week && i.Year >= year)
                                break;
                        }
                        break;
                }                

                return new Tuple<DateTime, DateTime?>(dateFrom, dateTo);
            }

        }

        public static DateTime? DateFromName(string date, IEnumerable<string> templates, out bool monthSpecified, out bool daySpecified)
        {
            var yearChar = AppSettings.Get("templateYearChar")[0];
            var monthChar = AppSettings.Get("templateMonthChar")[0];
            var dayChar = AppSettings.Get("templateDayChar")[0];
            var wildChar = AppSettings.Get("templateWildcardChar")[0];
            monthSpecified = false;
            daySpecified = false;


            if (templates == null)
                return null;

            foreach (var template in templates)
            {
                monthSpecified = false;
                daySpecified = false;

                if (template.Length != date.Length)
                    continue;

                try
                {
                    var monthStr = "";
                    var yearStr = "";
                    var dayStr = "";

                    for (var i = 0; i < template.Length; i++)
                    {
                        if (template[i] == monthChar)
                            monthStr += date[i];
                        else if (template[i] == yearChar)
                            yearStr += date[i];
                        else if (template[i] == dayChar)
                            dayStr += date[i];
                        else if (template[i] != wildChar && template[i] != date[i])
                            continue;
                    }

                    if (String.IsNullOrEmpty(monthStr))
                        monthStr = "01";
                    else
                        monthSpecified = true;

                    if (String.IsNullOrEmpty(dayStr))
                        dayStr = "01";
                    else
                        daySpecified = true;

                    var dateFrom = DateTime.ParseExact($"{monthStr}/{yearStr}", "MM/yyyy", CultureInfo.InvariantCulture);                   

                    return dateFrom;
                }
                catch (Exception ex)
                {
                    LogUtils.LogException(ex);
                    return null;
                }
            }

            return null;
        }

        public static object CreateWorkLogObject(int userId, float durationHrs, int logType, int itemId, string itemType, DateTime forDate, string description = null)
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
                //description = (description ?? "") + " (via OnTimeSpeed)",
                description = description,
                item = new
                {
                    id = itemId,
                    item_type = itemType
                },
                date_time = forDate.ToString("yyyy-MM-ddT12:00:00")
            };
        }

        public static object CreateWorkLogObjectForUpdate(float durationHrs, string description = null)
        {
            return new
            {               
                work_done = new
                {
                    duration = durationHrs,
                    time_unit = new
                    {
                        id = 2
                    }
                },
                description,
            };
        }


        public static WorkItem GetTaskForDate(
            List<WorkItem> items, 
            DateTime forDate, 
            string dateFromTemplateName, 
            string dateToTemplateName = null)
        {
            var templates = Templates.Get();

            foreach (var item in items)
            {
                item.Name = item.Name.Trim();

                var fromDate = DateFromName(item.Name, templates.FirstOrDefault(t => t.Name == dateFromTemplateName).Templates, out var dateFromMonthSpecified, out var dateFromDaySpecified);
                DateTime? toDate = null;

                if (String.IsNullOrEmpty(dateToTemplateName))
                    toDate = fromDate?.AddYears(1).AddDays(-1);
                else
                {
                    toDate = DateFromName(item.Name, templates.FirstOrDefault(t => t.Name == dateToTemplateName).Templates, out var dateToMonthSpecified, out var dateToDaySpecified);
                    if (toDate != null && !dateToMonthSpecified)
                    {
                        toDate = toDate.Value.AddYears(1).AddDays(-1);
                    }
                    if (toDate != null && !dateToDaySpecified)
                    {
                        toDate = toDate.Value.AddMonths(1).AddDays(-1);
                    }
                }

                if (fromDate == null || toDate == null)
                    continue;
                else
                {
                    if (fromDate <= forDate && forDate <= toDate)
                        return item;
                }
            }

            return null;
        }
    }
}