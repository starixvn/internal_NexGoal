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

        public async Task<IEnumerable<MemberModel>> GetSettingAsync(string pmIds)
        {
            return await baseService.GetDataAsync<MemberModel>(new GetPMSettingCriteria
            {
                PMIds = pmIds
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> AddMembers(int projectManagerId, string memberIds)
        {
            return await baseService.ExecuteNonQueryAsync(new AddMemberCriteria
            {
                ProjectManagerId = projectManagerId,
                MemberIds = memberIds
            }).ConfigureAwait(false);
        }
    }
}