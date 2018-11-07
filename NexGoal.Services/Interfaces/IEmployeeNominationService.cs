namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.PerformanceRecord;

    public interface IEmployeeNominationService
    {
        Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForNominator(int nominatorId);

        Task<CommandResult> Create(CreateEmployeeNominationModel model);

        Task<IEnumerable<PerformanceRecordModel>> GetMemberNominationssAsync(int nominatorId, int? memberId);

        Task<PerformanceRecordModel> GetNominationByRecordIdAsync(int performanceRecordId);
    }
}