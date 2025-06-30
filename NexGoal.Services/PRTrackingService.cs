namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Criteria.PRTracking;
    using NexGoal.Models.Common;
    using NexGoal.Models.Period;
    using NexGoal.Models.PRTracking;
    using NexGoal.Repositories;

    public class PRTrackingService(IPRTrackingRepository repository) : IPRTrackingService
    {
        public async Task<(PeriodModel, IEnumerable<PRTrackingModel>)> GetAsync(int actorId, string employeeIds, byte? noPR)
        {
            var criteria = new GetPRCriteria
            {
                ActorId = actorId,
                EmployeeIds = employeeIds,
                NoPR = noPR
            };
            return await repository.GetAsync(criteria);
        }

        public async Task<(IEnumerable<Group>, IEnumerable<Employee>)> GetGroupsAndMembersAsync(int actorId)
        {
            var criteria = new GetGroupsAndMembersCriteria
            {
                ActorId = actorId
            };
            return await repository.GetGroupsAndMembersAsync(criteria);
        }
    }
}