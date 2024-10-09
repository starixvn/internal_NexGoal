namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.Period;
    using NexGoal.Models.PRTracking;

    public interface IPRTrackingRepository
    {
        Task<(PeriodModel, IEnumerable<PRTrackingModel>)> GetAsync(ICriteria criteria);

        Task<(IEnumerable<Group>, IEnumerable<Employee>)> GetGroupsAndMembersAsync(ICriteria criteria);
    }
}