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
    public class LunchEntry : IAutomaticEntry
    {
        public bool CanAddWorkLog(List<WorkLog> logs, Dictionary<DateTime, string> vacationDays, WorkItem newItem, DateTime onDate, out float addAmount)
        {
            addAmount = 0;

            if (vacationDays.ContainsKey(onDate.Date) == false)
            {
                if (newItem == null || onDate.IsWeekend() || onDate.IsHoliday())
                    return false;

                var canAdd = true;
                var logsForDay = logs.Where(l => l.date_time.Date == onDate.Date);
                var workedOnDay = logsForDay.Sum(l => l.work_done.duration_minutes / 60);

                if (workedOnDay >= 7.5)
                    return false; //nema mjesta za dodati ručak

                foreach (var log in logsForDay)
                {
                    if (log.item.id == newItem.Id)
                    {
                        canAdd = false;
                        break;
                    }
                }

                addAmount = 0.5f;
                return canAdd;
            }
            else
                return false;
        }

        public object CreateWorkLogObj(int userId, int itemId, DateTime forDate, float addAmount, Dictionary<DateTime, string> vacationDays)
        {
            return PrepareData.CreateWorkLogObject(userId, addAmount, AppSettings.GetInt("rucakWorkLogType"), itemId, "tasks", forDate);
        }

        public async Task<List<WorkItem>> GetAllRelatedTasks(Models.User user)
        {
            string cacheKey = "lunchTasks";
            var searchStrings = SearchStrings.Get().FirstOrDefault(s => s.Name == "rucakSearchString").SearchStrings;
            var result = await DAL.GetWorkItems(user, searchStrings, new List<string> { "tasks" }, cacheKey);

            return result;
        }

        public async Task<Dictionary<DateTime, string>> GetApprovedVacationDays(HrNetMobile.Models.User hrproUser)
        {
            var dict = await DAL_HrProApi.GetApprovedVacationDays(hrproUser);
            var dict2 = await DAL_HrProApi.GetPaidLeaves(hrproUser);

            foreach (var d in dict2)
            {
                if (dict.ContainsKey(d.Key) == false)
                    dict.Add(d.Key, d.Value);
            }
            return dict;
        }

        public WorkItem GetTaskForDate(List<WorkItem> items, DateTime forDate)
        {
            return PrepareData.GetTaskForDate(items, forDate, "rucakDateFromTemplate", "rucakDateToTemplate");
        }
    }
}