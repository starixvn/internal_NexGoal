namespace NexGoal.BackgroundJob.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Models;

    public interface IWorkingTimeRequestService
    {
        Task<IEnumerable<WorkingTimeRequest>> GetWorkingTimeRequestsAsync(DateTime? lastModifiedDate = null);
        Task<DateTime?> GetLastModifiedDateAsync();
        Task<CommandResult> ImportWorkingTimeRequestsAsync(SyncWorkingTimeRequestCriteria criteria);
    }
} 