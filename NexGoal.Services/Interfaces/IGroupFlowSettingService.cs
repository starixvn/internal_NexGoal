namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.FlowSetting;

    public interface IGroupFlowSettingService
    {
        Task<IEnumerable<GroupBaseModel>> GetGroupHaveSettingAsync(int flowTempalteId);

        Task<IEnumerable<GroupFlowSettingModel>> GetGroupFlowSettingAsync(int flowTempalteId);

        Task<GroupFlowSettingModel> GetByIdAsync(int groupId, int flowTempalteId);

        Task<CommandResult> EditAsync(int groupId, int flowTemplateId, IEnumerable<FlowSettingModel> settings);

        Task<CommandResult> DeleteAsync(int groupId, int flowTemplateId);
    }
}