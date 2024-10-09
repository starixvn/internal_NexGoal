namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.Common;
    using NexGoal.Models.Period;
    using NexGoal.Models.PRTracking;

    public interface IPRTrackingService
    {
        Task<(PeriodModel, IEnumerable<PRTrackingModel>)> GetAsync(int actorId, string employeeIds, byte? noPR);

        Task<(IEnumerable<Group>, IEnumerable<Employee>)> GetGroupsAndMembersAsync(int actorId);
    }
}