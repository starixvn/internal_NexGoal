namespace NexGoal.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.FlowSetting;

    public interface IGroupFlowSettingService
    {
        Task<GroupFlowSettingModel> GetByIdAsync(int groupId, int flowTempalteId);

        Task<CommandResult> DeleteAsync(int groupId, int flowTemplateId);
    }
}