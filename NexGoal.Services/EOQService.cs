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
        private readonly ICommonService commonService;

        public EOQService(IBaseService baseService, ICommonService commonService)
        {
            this.baseService = baseService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<EOQYModel>> GetAllAsync(int? year = null, int? statusId = null)
        {
            var criteria = new GetEOQCriteria
            {
                Year = year,
                StatusId = statusId
            };

            return await baseService.GetDataAsync<EOQYModel>(criteria).ConfigureAwait(false);
        }

        public async Task<EOQYModel> GetByIdAsync(int eoqId)
        {
            var criteria = new GetEOQByIdCriteria
            {
                EOQId = eoqId
            };

            return await baseService.GetDataEntityAsync<EOQYModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<int>> GetYearsAsync()
        {
            var criteria = new GetYearsCriteria();

            return await baseService.GetDataAsync<int>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> CreateAsync(int actorId, CreateEOQModel eoq)
        {
            var criteria = new CreateEOQCriteria
            {
                Quarter = eoq.Quarter,
                Year = eoq.Year,
                StartDateForNominator = eoq.StartDateForNominator,
                EndDateForNominator = eoq.EndDateForNominator,
                RemindDateForNominator = eoq.RemindDateForNominator,
                EndDateForCommittee = eoq.EndDateForCommittee,
                Description = commonService.RemoveInvalidXmlChars(eoq.Description),
                CreatedBy = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditAsync(int actorId, EditEOQModel eoq)
        {
            var criteria = new EditEOQCriteria
            {
                EOQId = eoq.EOQId,
                Quarter = eoq.Quarter,
                Year = eoq.Year,
                StartDateForNominator = eoq.StartDateForNominator,
                EndDateForNominator = eoq.EndDateForNominator,
                RemindDateForNominator = eoq.RemindDateForNominator,
                EndDateForCommittee = eoq.EndDateForCommittee,
                Description = commonService.RemoveInvalidXmlChars(eoq.Description),
                ModifiedBy = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}