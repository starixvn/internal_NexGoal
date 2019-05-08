namespace NexGoal.Services
{
    using NexGoal.Criteria.PRTracking;
    using NexGoal.Models.Common;
    using NexGoal.Models.Period;
    using NexGoal.Models.PRTracking;
    using NexGoal.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class PRTrackingService : IPRTrackingService
    {
        private readonly IPRTrackingRepository repository;

        public PRTrackingService(IPRTrackingRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Tuple<PeriodModel, IEnumerable<PRTrackingModel>>> GetAsync(int actorId, string employeeIds, bool noPR)
        {
            return await repository.GetAsync(new GetPRCriteria
            {
                ActorId = actorId,
                EmployeeIds = employeeIds,
                NoPR = noPR
            });
        }

        public async Task<Tuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupsAndMembersAsync(int actorId)
        {
            return await repository.GetGroupsAndMembersAsync(new GetGroupsAndMembersCriteria
            {
                ActorId = actorId
            });
        }

        public async Task<IEnumerable<PRTrackingDetailModel>> GetDetailAsync(int employeeId, int processId)
        {
            return await repository.GetDetailAsync(new GetPRTrackingDetailCriteria
            {
                EmployeeId = employeeId,
                ProcessId = processId
            });
        }
    }
}