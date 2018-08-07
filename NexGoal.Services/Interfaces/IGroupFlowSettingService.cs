namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.FlowSetting;
    using NexGoal.Models.UserManagement;

    public interface IGroupFlowSettingService
    {
        Task<IEnumerable<UserGroupModel>> GetGroupHasSettingAsync(int flowTempalteId);

        Task<GroupFlowSettingModel> GetByIdAsync(int groupId, int flowTempalteId);

        Task<CommandResult> EditAsync(int groupId, int flowTemplateId, IEnumerable<FlowSettingModel> settings);

        Task<CommandResult> DeleteAsync(int groupId, int flowTemplateId);

        Task<UserModel> GetEmployeeByIdAsync(int employeeId);
    }
}