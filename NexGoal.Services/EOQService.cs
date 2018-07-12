namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EOQ;
    using NexGoal.Models.EOQY;

    public class EOQService : IEOQService
    {
        private readonly IBaseService baseService;

        public EOQService(IBaseService baseService)
        {
            this.baseService = baseService;
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

        public async Task<IEnumerable<int>> GetYearEOQAsync()
        {
            var criteria = new GetYearEOQCriteria();

            return await baseService.GetDataAsync<int>(criteria).ConfigureAwait(false);
        }
    }
}