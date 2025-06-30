namespace NexGoal.BackgroundJob.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Models;

    public interface IEmployeeOvertimeService
    {
        Task<IEnumerable<EmployeeOvertime>> GetOvertimesAsync(DateTime? lastModifiedDate = null);
        Task<DateTime?> GetLastModifiedDateAsync();
        Task<CommandResult> ImportOvertimesAsync(SyncEmployeeOvertimeCriteria criteria);
    }
} 