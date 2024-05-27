namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.Common;
    using NexGoal.Models.Period;
    using NexGoal.Models.PRTracking;

    public interface IPRTrackingService
    {
        Task<ValueTuple<PeriodModel, IEnumerable<PRTrackingModel>>> GetAsync(int actorId, string employeeIds, byte? noPR);

        Task<ValueTuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupsAndMembersAsync(int actorId);
    }
}