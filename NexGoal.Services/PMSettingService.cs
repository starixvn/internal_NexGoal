namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PMSetting;
    using NexGoal.Models.Common;
    using NexGoal.Models.PMSetting;

    public class PMSettingService : IPMSettingService
    {
        private readonly IBaseService baseService;

        public PMSettingService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<MemberModel>> GetSettingAsync(string projectManagerIds, int? employeeId = null)
        {
            var criteria = new GetPMSettingCriteria
            {
                ProjectManagerIds = projectManagerIds,
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<MemberModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> AddMemberAsync(int projectManagerId, string memberIds, int actorId)
        {
            var criteria = new AddMemberCriteria
            {
                ProjectManagerId = projectManagerId,
                MemberIds = memberIds,
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> RemoveMemberAsync(int projectManagerId, int memberId, int actorId)
        {
            var criteria = new RemoveMemberCriteria
            {
                ProjectManagerId = projectManagerId,
                MemberId = memberId,
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Employee>> GetProjectManagerAsync()
        {
            return await baseService.GetDataAsync<Employee>(new GetProjectManagersCriteria()).ConfigureAwait(false);
        }
    }
}