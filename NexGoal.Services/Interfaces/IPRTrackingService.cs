namespace NexGoal.Services
{
    using NexGoal.Models.Common;
    using NexGoal.Models.Period;
    using NexGoal.Models.PRTracking;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPRTrackingService
    {
        Task<Tuple<PeriodModel, IEnumerable<PRTrackingModel>>> GetAsync(int actorId, string employeeIds, bool noPR);

        Task<Tuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupsAndMembersAsync(int actorId);
    }
}