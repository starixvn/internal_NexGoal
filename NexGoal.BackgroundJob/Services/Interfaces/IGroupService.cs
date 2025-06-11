namespace NexGoal.BackgroundJob.Services
{
    using System.Threading.Tasks;
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Models;

    public interface IGroupService
    {
        Task<IEnumerable<Group>> GetGroupsAsync(DateTime? lastModifiedDate);

        Task<CommandResult> ImportGroupsAsync(SyncGroupCriteria criteria);

        Task<DateTime?> GetLastModifiedDateAsync();

    }
}