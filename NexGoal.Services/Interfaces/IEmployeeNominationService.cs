namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.PerformanceRecord;

    public interface IEmployeeNominationService
    {
        Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForNominator(int nominatorId);

        Task<CommandResult> Create(CreateEmployeeNominationModel model);

        Task<Tuple<IEnumerable<PerformanceRecordModel>, int>> GetMemberNominationssAsync(int nominatorId, int? memberId, int pageSize, int page);

        Task<PerformanceRecordModel> GetNominationByRecordIdAsync(int performanceRecordId);

        Task<bool> IsNominator(int actorId);
    }
}