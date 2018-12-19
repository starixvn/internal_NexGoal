namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.NominationReport;
    using NexGoal.Models.NominationPeriod;

    public class NominationReportService : INominationReportService
    {
        private readonly IBaseService baseService;

        public NominationReportService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForNominatorAsync(bool isQuarter)
        {
            var criteria = new GetNominationPeriods
            {
                IsQuarter = isQuarter
            };

            return await baseService.GetDataAsync<NominationPeriodModel>(criteria).ConfigureAwait(false);
        }
    }
}