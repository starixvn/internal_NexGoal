namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.NominationPeriod;
    using NexGoal.Models.NominationPeriod;

    public class NominationPeriodService : INominationPeriodService
    {
        private readonly IBaseService baseService;

        public NominationPeriodService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<NominationPeriodModel>> GetAsync(bool? isCompleted = null)
        {
            var criteria = new GetNominationPeriodCriteria
            {
                IsCompleted = isCompleted
            };

            return await baseService.GetDataAsync<NominationPeriodModel>(criteria);
        }

        public async Task<CommandResult> DeleteAsync(int nominationPeriodId, int actorId)
        {
            var criteria = new DeleteNominationPeriodCriteria
            {
                NominationPeriodId = nominationPeriodId,
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<IEnumerable<RoleSettingModel>> GetRoleSettingAsync(GetRoleSettingParam param)
        {
            var criteria = new GetRoleSettingCriteria
            {
                NominationPeriodId = param.NominationPeriodId,
                Keyword = param.Keyword,
                GroupIds = param.GroupIds,
                RoleId = param.RoleId,
                IsShowAll = param.IsShowAll
            };

            return await baseService.GetDataAsync<RoleSettingModel>(criteria);
        }

        public async Task<CommandResult> EditRoleSettingAsync(EditRoleSettingModel setting, int actorId)
        {
            var criteria = new EditRoleSettingCriteria
            {
                NominationPeriodId = setting.NominationPeriodId,
                EmployeeId = setting.EmployeeId,
                RoleIds = setting.RoleIds,
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}