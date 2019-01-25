namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.Discipline;
    using NexGoal.Models.PARecord;

    public interface IPARecordService
    {
        Task<IEnumerable<PeriodBaseModel>> GetPeriodByActorAsync(int actorId);

        Task<IEnumerable<GroupBaseModel>> GetGroupByActorAsync(int actorId, int periodId);

        Task<IEnumerable<PerformanceAppraisalResult>> GetPerformanceAppraisalResultAsync(int employeeId);

        Task<IEnumerable<PerformanceAppraisalResult>> GetMemberPerformanceAppraisalResultAsync(
            int actorId,
            int? periodId = null,
            int? employeeId = null);

        Task<IEnumerable<PARecordModel>> GetPARecordByReviewerAsync(GetPARecordParam param);

        Task<PARecordModel> GetPARecordByIdAsync(int paRecordId, int actorId);

        Task<CommandResult> EditPARecordByReviewerAsync(int actorId, EditPARecordByReviewerModel paRecord);

        Task<CommandResult> QuickEditPARecordByReviewerAsync(int actorId, int paRecordId, bool isRevised);

        Task<Policy> GetPermissionOnPerformanceAppraisalAsync(int actorId, int recordId);

        Task<IEnumerable<DisciplineModel>> GetDisciplineByRecordIdAsync(int paRecordId);

        Task<string> GetPreviousPAGrade(int currentPeriodId, int actorId, int employeeId);
    }
}