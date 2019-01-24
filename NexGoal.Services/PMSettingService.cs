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

        public async Task<IEnumerable<MemberModel>> GetSettingAsync(string projectManagerIds, int? employeeId)
        {
            var criteria = new GetPMSettingCriteria
            {
                ProjectManagerIds = projectManagerIds,
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<MemberModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> AddMembers(int projectManagerId, string memberIds, int actorId)
        {
            var criteria = new AddMemberCriteria
            {
                ProjectManagerId = projectManagerId,
                MemberIds = memberIds,
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> RemoveMember(int projectManagerId, int memberId, int actorId)
        {
            var criteria = new RemoveMemberCriteria
            {
                ProjectManagerId = projectManagerId,
                MemberId = memberId,
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<EmployeeBaseModel>> GetProjectManagerAsync()
        {
            var criteria = new GetProjectManagersCriteria();

            return await baseService.GetDataAsync<EmployeeBaseModel>(criteria).ConfigureAwait(false);
        }
    }
}