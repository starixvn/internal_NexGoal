namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Criteria.PRTracking;
    using NexGoal.Models.Common;
    using NexGoal.Models.Period;
    using NexGoal.Models.PRTracking;
    using NexGoal.Repositories;

    public class PRTrackingService : IPRTrackingService
    {
        private readonly IPRTrackingRepository repository;

        public PRTrackingService(IPRTrackingRepository repository)
        {
            this.repository = repository;
        }

        public async Task<ValueTuple<PeriodModel, IEnumerable<PRTrackingModel>>> GetAsync(int actorId, string employeeIds, int? noPR)
        {
            return await repository.GetAsync(new GetPRCriteria
            {
                ActorId = actorId,
                EmployeeIds = employeeIds,
                NoPR = noPR
            });
        }

        public async Task<ValueTuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupsAndMembersAsync(int actorId)
        {
            return await repository.GetGroupsAndMembersAsync(new GetGroupsAndMembersCriteria
            {
                ActorId = actorId
            });
        }
    }
}