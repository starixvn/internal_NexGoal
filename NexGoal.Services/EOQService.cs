namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EOQ;
    using NexGoal.Models.EOQY;

    public class EOQService : IEOQService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;

        public EOQService(
            IBaseService baseService,
            ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
        }

        public async Task<IEnumerable<EOQYModel>> GetAllEOQAsync(int? year = null, int? statusId = null)
        {
            var criteria = new GetEOQCriteria
            {
                Year = year,
                StatusId = statusId
            };

            return await baseService.GetDataAsync<EOQYModel>(criteria).ConfigureAwait(false);
        }

        public async Task<EOQYModel> GetEOQByIdAsync(int eoqId)
        {
            var criteria = new GetEOQByIdCriteria
            {
                EOQYId = eoqId
            };

            return await baseService.GetDataEntityAsync<EOQYModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<int>> GetYearsAsync()
        {
            var criteria = new GetYearsCriteria();

            return await baseService.GetDataAsync<int>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> CreateEOQAsync(CreateEOQModel eoq)
        {
            var criteria = new CreateEOQCriteria
            {
                Quarter = eoq.Quarter,
                Year = eoq.Year,
                StartDateForNominator = eoq.StartDateForNominator,
                EndDateForNominator = eoq.EndDateForNominator,
                RemindDateForNominator = eoq.RemindDateForNominator,
                EndDateForCommittee = eoq.EndDateForCommittee,
                Description = eoq.Description,
                CreatedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditEOQAsync(EOQYModel eoq)
        {
            var criteria = new EditEOQCriteria
            {
                EOQYId = eoq.EOQYId,
                Quarter = eoq.Quarter,
                Year = eoq.Year,
                StartDateForNominator = eoq.StartDateForNominator,
                EndDateForNominator = eoq.EndDateForNominator,
                RemindDateForNominator = eoq.RemindDateForNominator,
                EndDateForCommittee = eoq.EndDateForCommittee,
                Description = eoq.Description,
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}