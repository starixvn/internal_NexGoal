namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.FlowSetting;

    public interface IEmployeeFlowSettingService
    {
        Task<IEnumerable<EmployeeBaseModel>> GetEmployeeHaveSettingAsync(int? groupId = null);

        Task<IEnumerable<EmployeeBaseModel>> GetEmployeeUnsettingAsync(int? groupId = null);

        Task<IEnumerable<EmployeeFlowSettingModel>> GetEmployeeFlowSettingAsync();

        Task<EmployeeFlowSettingModel> GetByIdAsync(int employeeId);

        Task<CommandResult> EditAsync(int actorId, int employeeId, IEnumerable<FlowSettingModel> settings);

        Task<CommandResult> DeleteAsync(int actorId, int employeeId);
    }
}