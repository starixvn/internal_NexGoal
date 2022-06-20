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
        Task<CommandResult> CreateAsync(CreateEmployeeNominationModel model);

        Task<CommandResult> EditAsync(EditEmployeeNominationModel model);

        Task<CommandResult> DeleteAsync(int performanceRecordId, int deletedById);

        Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForNominatorAsync(int nominatorId);

        Task<IEnumerable<EmployeeNominationModel>> GetNominationResultAsync(int employeeId);

        Task<IEnumerable<EmployeeNominationModel>> GetMemberNominationResultAsync(int actorId, int? employeeId = null, int? year = null);

        Task<IEnumerable<PerformanceRecord>> GetMemberNominationAsync(int nominatorId);

        Task<IEnumerable<PerformanceRecord>> GetMemberNominationByIdAsync(int nominatorId, int memberId);

        Task<PerformanceRecord> GetNominationByRecordIdAsync(int performanceRecordId);

        Task<NominationPeriodModel> GetCurrentNominationPeriodForComitteeAsync(int committeeId, bool isQuarter);

        Task<IEnumerable<NominationModel>> GetNominationsByPeriodIdsAsync(string nominationPeriodIds, int? statusId = null);

        Task<CommandResult> EditStatusAsync(int nominationId, int statusId, int updatedById);

        Task<IEnumerable<TemplateKPINominationInfoModel>> GetTemplateKIPNominationAsync();
    }
}