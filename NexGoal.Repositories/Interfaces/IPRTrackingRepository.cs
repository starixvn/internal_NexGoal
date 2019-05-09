namespace NexGoal.Repositories
{
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.Period;
    using NexGoal.Models.PRTracking;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPRTrackingRepository
    {
        Task<Tuple<PeriodModel, IEnumerable<PRTrackingModel>>> GetAsync(ICriteria criteria);

        Task<Tuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupsAndMembersAsync(ICriteria criteria);
    }
}