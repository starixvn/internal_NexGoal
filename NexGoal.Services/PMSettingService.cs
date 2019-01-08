namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PMSetting;
    using NexGoal.Models.PMSetting;

    public class PMSettingService : IPMSettingService
    {
        private readonly IBaseService baseService;

        public PMSettingService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<MemberModel>> GetSettingAsync(string pmIds, int? employeeId)
        {
            return await baseService.GetDataAsync<MemberModel>(new GetPMSettingCriteria
            {
                PMIds = pmIds,
                EmployeeId = employeeId
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> AddMembers(int projectManagerId, string memberIds, int createdBy)
        {
            return await baseService.ExecuteNonQueryAsync(new AddMemberCriteria
            {
                ProjectManagerId = projectManagerId,
                MemberIds = memberIds,
                CreatedBy = createdBy
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> RemoveMember(int projectManagerId, int memberId, int removedBy)
        {
            return await baseService.ExecuteNonQueryAsync(new RemoveMemberCriteria
            {
                ProjectManagerId = projectManagerId,
                MemberId = memberId,
                DeletedBy = removedBy
            }).ConfigureAwait(false);
        }
    }
}