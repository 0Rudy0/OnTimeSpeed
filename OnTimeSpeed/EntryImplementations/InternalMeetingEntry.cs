using OnTimeSpeed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using hrnetModel = HrNetMobile.Models;
using OnTimeSpeed.Utils;
using OnTimeSpeed.Code;

namespace OnTimeSpeed.EntryImplementations
{
    public class InternalMeetingEntry : ISemiAutomaticEntry
    {
        public async Task<List<WorkItem>> GetAllRelatedTasks(User user)
        {
            string cacheKey = "internalMeetingTasks";
            var searchStrings = SearchStrings.Get().FirstOrDefault(s => s.Name == "interniSastanakSearchString").SearchStrings;
            var result = await DAL.GetWorkItems(user, searchStrings, new List<string> { "tasks" }, cacheKey);

            return result;
        }

        public WorkItem GetTaskForDate(List<WorkItem> items, DateTime forDate)
        {
            return PrepareData.GetTaskForDate(items, forDate, "interniSastanakTemplate");
        }

        public object CreateWorkLogObj(int userId, int itemId, DateTime forDate, float addAmount, string description)
        {
            return PrepareData.CreateWorkLogObject(userId, addAmount, AppSettings.GetInt("interniSastanakWorkType"), itemId, "tasks", forDate, description);
        }
    }
}