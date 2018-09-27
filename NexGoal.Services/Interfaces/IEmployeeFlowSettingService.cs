namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.FlowSetting;

    public interface IEmployeeFlowSettingService
    {
        Task<IEnumerable<EmployeeBaseModel>> GetEmployeeHaveSettingAsync(int flowTempalteId, int? groupId = null);

        Task<IEnumerable<EmployeeBaseModel>> GetEmployeeUnsettingAsync(int flowTempalteId, int? groupId = null);

        Task<IEnumerable<EmployeeFlowSettingModel>> GetEmployeeFlowSettingAsync(int flowTempalteId);

        Task<EmployeeFlowSettingModel> GetByIdAsync(int employeeId, int flowTempalteId);

        Task<CommandResult> EditAsync(int employeeId, int flowTemplateId, IEnumerable<FlowSettingModel> settings);

        Task<CommandResult> DeleteAsync(int employeeId, int flowTemplateId);
    }
}