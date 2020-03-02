using OnTimeSpeed.Models;
using OnTimeSpeed.Utils;
using OnTimeSpeed.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OnTimeSpeed.EntryImplementations
{
    public class HolidayEntry : IAutomaticEntry
    {
        public bool CanAddWorkLog(List<WorkLog> logs, Dictionary<DateTime, string> vacationDays, WorkItem newItem, DateTime onDate, float lunchAmount, out float addAmount)
        {
            addAmount = 0;

            if (newItem == null || onDate.IsWeekend() || !onDate.IsHoliday())
                return false;

            var logsForDay = logs.Where(l => l.date_time.Date == onDate.Date);
            var workedOnDay = logsForDay.Sum(l => l.work_done.duration_minutes / 60);

            if (workedOnDay >= 8)
                return false; //nema mjesta za dodati praznik, očito se radilo već taj dan

            foreach (var log in logsForDay)
            {
                if (log.item.id == newItem.Id)
                {
                    return false;
                }
            }

            addAmount = 8 - workedOnDay;
            return addAmount > 0;
        }

        public object CreateWorkLogObj(int userId, int itemId, DateTime forDate, float addAmount, Dictionary<DateTime, string> vacationDays)
        {
            return PrepareData.CreateWorkLogObject(userId, addAmount, AppSettings.GetInt("praznikWorkType"), itemId, "tasks", forDate, forDate.HolidayName());
        }

        public async Task<List<WorkItem>> GetAllRelatedTasks(Models.User user)
        {
            string cacheKey = "holidayTasks";
            var searchStrings = SearchStrings.Get().FirstOrDefault(s => s.Name == "holidaySearchString").SearchStrings;
            var result = await DAL.GetWorkItems(user, searchStrings, new List<string> { "tasks" }, cacheKey);

            return result;
        }

        public async Task<Dictionary<DateTime, string>> GetApprovedVacationDays(HrNetMobile.Models.User hrproUser)
        {
            return new Dictionary<DateTime, string>();
        }

        public string GetEntryDescription()
        {
            return "Praznik";
        }

        public WorkItem GetTaskForDate(List<WorkItem> items, DateTime forDate)
        {
            return PrepareData.GetTaskForDate(items, forDate, "praznikTemplate");
        }
    }
}