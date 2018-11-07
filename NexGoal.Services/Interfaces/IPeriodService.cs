namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Period;

    public interface IPeriodService
    {
        Task<IEnumerable<PeriodModel>> GetPeriodAsync(bool? isActive = null);

        Task<PeriodModel> GetByIdAsync(int periodId);

        Task<CommandResult> CreateAsync(CreatePeriodModel model);

        Task<CommandResult> EditStatusAsync(int periodId, bool isActive);

        Task<CommandResult> DeleteAsync(int periodId);

        Task<CommandResult> EditAsync(EditPeriodModel model);

        Task<IEnumerable<PeriodProcessSettingModel>> GetPeriodProcessSettingAsync(int periodId);

        Task<CommandResult> EditProcessSettingAsync(int periodId, IEnumerable<EditProcessSettingModel> settings);

        Task<IEnumerable<PARecordModel>> GetEmployeeSettingAsync(int periodId, int? groupId = null, bool? isEvaluated = null, int? statusId = null);

        Task<CommandResult> CreateEmployeeSettingAsync(int periodId, string groupIds, int? employeeId = null);

        Task<CommandResult> EditEmployeeSettingAsync(int periodId, int[] paRecordIds, IEnumerable<EditEmployeeSettingModel> processes);

        Task<CommandResult> EditEmployeeSettingStatusAsync(int paRecordId, int statusId);

        Task<PARecordModel> GetEmployeeSettingByIdAsync(int paRecordId);

        Task<CommandResult> GeneratePAAsync(int periodId, string paRecordIds, int createdBy);
    }
}