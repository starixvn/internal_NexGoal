namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PerformanceRecord;

    public interface IEmployeeNominationService
    {
        Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForNominatorAsync(int nominatorId);

        Task<CommandResult> CreateAsync(CreateEmployeeNominationModel model);

        Task<IEnumerable<PerformanceRecordModel>> GetMemberNominationsAsync(int nominatorId, int? memberId);

        Task<PerformanceRecordModel> GetNominationByRecordIdAsync(int performanceRecordId);

        Task<CommandResult> EditAsync(EditEmployeeNominationModel model);

        Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForComitteeAsync(int committeeId);

        Task<IEnumerable<NominationModel>> GetNominationsByPeriodIdsAsync(string nominationPeriodIds);

        Task<CommandResult> EditStatusAsync(int nominationId, int statusId, int updatedById);

        Task<CommandResult> DeleteAsync(int performanceRecordId, int deletedById);
    }
}