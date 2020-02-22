using OnTimeSpeed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hrnetModel = HrNetMobile.Models;

namespace OnTimeSpeed.EntryImplementations
{
    public interface IAutomaticEntry
    {
        Task<List<WorkItem>> GetAllRelatedTasks(User user);

        Task<Dictionary<DateTime, string>> GetApprovedVacationDays(hrnetModel.User hrproUser);

        WorkItem GetTaskForDate(List<WorkItem> items, DateTime forDate);

        bool CanAddWorkLog(List<WorkLog> logs, Dictionary<DateTime, string> vacationDays, WorkItem newItem, DateTime onDate, out float addAmount);

        object CreateWorkLogObj(int userId, int itemId, DateTime forDate, float addAmount, Dictionary<DateTime, string> vacationDays);
    }
}
