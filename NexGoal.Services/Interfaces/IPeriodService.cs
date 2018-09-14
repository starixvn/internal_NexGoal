namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Period;

    public interface IPeriodService
    {
        Task<IEnumerable<PeriodModel>> GetPeriods(bool? isActive);

        Task<PeriodModel> GetPeriod(int periodId);

        Task<CommandResult> Create(CreatePeriodModel model);

        Task<CommandResult> EditStatus(int periodId, bool isActive);

        Task<CommandResult> Delete(int periodId);

        Task<CommandResult> Edit(EditPeriodModel model);

        Task<IEnumerable<PeriodProcessSettingModel>> GetPeriodProcessSetting(int periodId);

        Task<CommandResult> EditProcessSetting(int periodId, IEnumerable<EditProcessSettingModel> settings);

        Task<IEnumerable<PARecordModel>> GetEmployeeSettings(int periodId, int? groupId, bool? isEvaluated, int? statusId);

        Task<CommandResult> CreateEmployeeSettings(int periodId, string groupIds, int? employeeId);

        Task<CommandResult> EditEmployeeSettings(int periodId, int[] paRecordIds, IEnumerable<EditEmployeeSettingModel> processes);

        Task<CommandResult> EditEmployeeSettingStatus(int paRecordId, int statusId);

        Task<PARecordModel> GetEmployeeSetting(int paRecordId);
    }
}