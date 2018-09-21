namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EmployeeNomination;
    using NexGoal.Models.EmployeeNomination;

    public class EmployeeNominationService : IEmployeeNominationService
    {
        private readonly IBaseService baseService;

        public EmployeeNominationService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForNominator(int nominatorId)
        {
            var criteria = new GetNominationPeriodsForNominatorCriteria
            {
                NominatorId = nominatorId
            };

            return await baseService.GetDataAsync<NominationPeriodModel>(criteria).ConfigureAwait(false);
        }
    }
}