namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.NominationPeriod;

    public interface INominationReportService
    {
        Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForNominatorAsync(bool isQuarter);
    }
}