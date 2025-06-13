namespace NexGoal.BackgroundJob.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Models;

    public interface IJobTitleService
    {
        Task<IEnumerable<JobTitle>> GetJobTitlesAsync(DateTime? lastModifiedDate = null);
        Task<CommandResult> ImportJobTitlesAsync(SyncJobTitleCriteria criteria);
    }
} 