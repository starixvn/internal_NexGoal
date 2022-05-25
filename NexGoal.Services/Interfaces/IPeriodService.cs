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

        Task<CommandResult> CreateAsync(int actorId, CreatePeriodModel model);

        Task<CommandResult> EditStatusAsync(int actorId, int periodId, bool isActive);

        Task<CommandResult> DeleteAsync(int actorId, int periodId);

        Task<CommandResult> EditAsync(int actorId, EditPeriodModel model);

        Task<IEnumerable<PeriodProcessSettingModel>> GetPeriodProcessSettingAsync(int periodId);

        Task<CommandResult> EditProcessSettingAsync(int actorId, int periodId, IEnumerable<EditProcessSettingModel> settings);

        Task<IEnumerable<PARecordModel>> GetEmployeeSettingAsync(int periodId, int? groupId = null, bool? isEvaluated = null, int? statusId = null);

        Task<CommandResult> CreateEmployeeSettingAsync(int actorId, int periodId, string groupIds, int? employeeId = null);

        Task<CommandResult> EditEmployeeSettingAsync(int actorId, int periodId, int[] paRecordIds, IEnumerable<EditEmployeeSettingModel> processes);

        Task<CommandResult> EditEmployeeSettingStatusAsync(int actorId, int paRecordId, bool isExcluded);

        Task<PARecordModel> GetEmployeeSettingByIdAsync(int paRecordId);

        Task<CommandResult> GeneratePAAsync(int periodId, string paRecordIds, int createdBy);

        Task<CommandResult> EditPAGradeAsync(string paRecordIds);
    }
}