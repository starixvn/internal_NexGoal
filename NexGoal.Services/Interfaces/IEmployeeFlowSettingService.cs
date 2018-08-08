namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.FlowSetting;
    using NexGoal.Models.UserManagement;

    public interface IEmployeeFlowSettingService
    {
        Task<IEnumerable<UserModel>> GetEmployeeHaveSettingAsync(int flowTempalteId, int? groupId = null);

        Task<IEnumerable<UserModel>> GetEmployeeDoNotHaveSettingAsync(int flowTempalteId, int? groupId = null);

        Task<EmployeeFlowSettingModel> GetByIdAsync(int employeeId, int flowTempalteId);

        Task<CommandResult> EditAsync(int employeeId, int flowTemplateId, IEnumerable<FlowSettingModel> settings);

        Task<CommandResult> DeleteAsync(int employeeId, int flowTemplateId);
    }
}