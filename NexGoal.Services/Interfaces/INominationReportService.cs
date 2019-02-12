namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;

    public interface INominationReportService
    {
        Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodAsync();

        Task<IEnumerable<NominationModel>> GetNominationAsync(
            int actorId,
            string nominationPeriodIds,
            string groupIds,
            string employeeIds,
            int? statusId);
    }
}