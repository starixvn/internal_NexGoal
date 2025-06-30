namespace NexGoal.BackgroundJob.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Models;

    public interface ITOEICService
    {
        Task<IEnumerable<TOEIC>> GetTOEICsAsync(DateTime? lastModifiedDate = null);
        Task<DateTime?> GetLastModifiedDateAsync();
        Task<CommandResult> ImportTOEICsAsync(SyncTOEICCriteria criteria);
    }
} 