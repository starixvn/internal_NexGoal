namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EOY;
    using NexGoal.Models.EOQY;

    public class EOYService : IEOYService
    {
        private readonly IBaseService baseService;

        public EOYService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<EOQYModel>> GetAllAsync(int? statusId = null)
        {
            var criteria = new GetEOYCriteria
            {
                StatusId = statusId
            };

            return await baseService.GetDataAsync<EOQYModel>(criteria);
        }

        public async Task<EOQYModel> GetByIdAsync(int eoyId)
        {
            var criteria = new GetEOYByIdCriteria
            {
                EOYId = eoyId
            };

            return await baseService.GetDataEntityAsync<EOQYModel>(criteria);
        }

        public async Task<CommandResult> CreateAsync(int actorId, CreateEOYModel eoy)
        {
            var criteria = new CreateEOYCriteria
            {
                Year = eoy.Year,
                StartDateForNominator = eoy.StartDateForNominator,
                EndDateForNominator = eoy.EndDateForNominator,
                RemindDateForNominator = eoy.RemindDateForNominator,
                EndDateForCommittee = eoy.EndDateForCommittee,
                Description = eoy.Description,
                CreatedBy = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<CommandResult> EditAsync(int actorId, EditEOYModel eoy)
        {
            var criteria = new EditEOYCriteria
            {
                EOYId = eoy.EOYId,
                Year = eoy.Year,
                StartDateForNominator = eoy.StartDateForNominator,
                EndDateForNominator = eoy.EndDateForNominator,
                RemindDateForNominator = eoy.RemindDateForNominator,
                EndDateForCommittee = eoy.EndDateForCommittee,
                Description = eoy.Description,
                ModifiedBy = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}