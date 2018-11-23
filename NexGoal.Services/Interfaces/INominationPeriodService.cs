namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.NominationPeriod;

    public interface INominationPeriodService
    {
        Task<IEnumerable<NominationPeriodModel>> GetAsync(bool? isCompleted = null);

        Task<CommandResult> DeleteAsync(int nominationPeriodId, int actorId);

        Task<IEnumerable<RoleSettingModel>> GetRoleSettingAsync(GetRoleSettingParam param);

        Task<CommandResult> EditRoleSettingAsync(EditRoleSettingModel setting, int actorId);
    }
}