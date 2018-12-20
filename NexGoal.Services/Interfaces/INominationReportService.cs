namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;

    public interface INominationReportService
    {
        Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForNominatorAsync(bool isQuarter);

        Task<IEnumerable<NominationModel>> GetNominations(int? nominationPeriodId, string groupIds, int? employeeId, int? statusId);
    }
}