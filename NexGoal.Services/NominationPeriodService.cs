namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.NominationPeriod;
    using NexGoal.Models.NominationPeriod;

    public class NominationPeriodService : INominationPeriodService
    {
        private readonly IBaseService baseService;

        public NominationPeriodService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<NominationPeriodModel>> GetAsync(bool? isCompleted = null)
        {
            var criteria = new GetNominationPeriodCriteria
            {
                IsCompleted = isCompleted
            };

            return await baseService.GetDataAsync<NominationPeriodModel>(criteria).ConfigureAwait(false);
        }
    }
}