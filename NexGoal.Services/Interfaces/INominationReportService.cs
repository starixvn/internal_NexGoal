namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;

    public interface INominationReportService
    {
        Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForNominatorAsync();

        Task<IEnumerable<NominationModel>> GetNominations(string nominationPeriodIds, string groupIds, int? employeeId, int? statusId, int loggedEmployeeId);
    }
}