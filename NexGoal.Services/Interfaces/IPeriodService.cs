namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Period;

    public interface IPeriodService
    {
        Task<IEnumerable<PeriodModel>> GetPeriods(int? targetTypeId, bool? isActive);

        Task<PeriodModel> GetPeriod(int periodId);

        Task<CommandResult> Create(CreatePeriodModel model);

        Task<CommandResult> EditStatus(int periodId, bool isActive, int modifiedBy);

        Task<CommandResult> Delete(int periodId, int modifiedBy);

        Task<CommandResult> Edit(EditPeriodModel model);

        Task<IEnumerable<PeriodProcessSettingModel>> GetPeriodProcessSetting(int periodId);

        Task<CommandResult> EditProcessSetting(int periodId, IEnumerable<EditProcessSettingModel> settings, int modifiedBy);

        Task<IEnumerable<PARecordModel>> GetEmployeeSettings(int periodId, int? groupId, bool? isLocal, bool? isEvaluated, int? statusId);

        Task<CommandResult> CreateEmployeeSettings(int periodId, string groupIds, int? employeeId);

        Task<CommandResult> EditEmployeeSettings(int[] paRecordIds, IEnumerable<EditEmployeeSettingModel> processes, int modifiedBy);
    }
}