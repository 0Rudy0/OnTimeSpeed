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
    public class VacationEntry : IAutomaticEntry
    {
        public bool CanAddWorkLog(List<WorkLog> logs, Dictionary<DateTime, string> vacationDays, WorkItem newItem, DateTime onDate, float lunchAmount, out float addAmount)
        {
            addAmount = 0;

            if (vacationDays.ContainsKey(onDate.Date))
            {
                if (newItem == null || onDate.IsWeekend() || onDate.IsHoliday())
                    return false;

                var logsForDay = logs.Where(l => l.date_time.Date == onDate.Date);
                var workedOnDay = logsForDay.Sum(l => l.work_done.duration_minutes / 60);

                if (workedOnDay >= 8)
                    return false; //nema mjesta za dodati GO, očito se radilo već taj dan

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
            else
                return false;
        }

        public object CreateWorkLogObj(int userId, int itemId, DateTime forDate, float addAmount, Dictionary<DateTime, string> vacationDays)
        {
            return PrepareData.CreateWorkLogObject(userId, addAmount, AppSettings.GetInt("godisnjiWorkType"), itemId, "tasks", forDate);
        }

        public async Task<List<WorkItem>> GetAllRelatedTasks(Models.User user)
        {
            string cacheKey = "vacationTasks";
            var searchStrings = SearchStrings.Get().FirstOrDefault(s => s.Name == "vacationSearchString").SearchStrings;
            var result = await DAL.GetWorkItems(user, searchStrings, new List<string> { "tasks" }, cacheKey, false);

            return result;
        }

        public async Task<Dictionary<DateTime, string>> GetApprovedVacationDays(HrNetMobile.Models.User hrproUser)
        {
            return await DAL_HrProApi.GetApprovedVacationDays(hrproUser);
        }

        public string GetEntryDescription()
        {
            return "GO";
        }

        public WorkItem GetTaskForDate(List<WorkItem> items, DateTime forDate)
        {
            return PrepareData.GetTaskForDate(items, forDate, "godisnjiTemplate");
        }
    }
}