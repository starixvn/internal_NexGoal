namespace NexGoal.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.Period;
    using NexGoal.Models.PRTracking;

    public interface IPRTrackingRepository
    {
        Task<ValueTuple<PeriodModel, IEnumerable<PRTrackingModel>>> GetAsync(ICriteria criteria);

        Task<ValueTuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupsAndMembersAsync(ICriteria criteria);
    }
}