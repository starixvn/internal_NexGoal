namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.FlowSetting;

    public interface IGroupFlowSettingService
    {
        Task<IEnumerable<Group>> GetGroupHaveSettingAsync();

        Task<IEnumerable<GroupFlowSettingModel>> GetGroupFlowSettingAsync();

        Task<GroupFlowSettingModel> GetByIdAsync(int groupId);

        Task<CommandResult> EditAsync(int actorId, int groupId, IEnumerable<FlowSettingModel> settings);

        Task<CommandResult> DeleteAsync(int actorId, int groupId);

        Task<IEnumerable<GroupModel>> GetGroupsAsync();
    }
}