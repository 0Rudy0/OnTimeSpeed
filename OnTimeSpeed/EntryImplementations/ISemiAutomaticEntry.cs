using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnTimeSpeed.Models;

namespace OnTimeSpeed.EntryImplementations
{
    public interface ISemiAutomaticEntry
    {
        Task<List<WorkItem>> GetAllRelatedTasks(User user);

        WorkItem GetTaskForDate(List<WorkItem> items, DateTime forDate);

        object CreateWorkLogObj(int userId, int itemId, DateTime forDate, float addAmount, string description);
    }
}
