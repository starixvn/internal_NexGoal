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
        private readonly ISessionService sessionService;

        public EOYService(
            IBaseService baseService,
            ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
        }

        public async Task<IEnumerable<EOQYModel>> GetAllEOYAsync(int? statusId = null)
        {
            var criteria = new GetEOYCriteria
            {
                StatusId = statusId
            };

            return await baseService.GetDataAsync<EOQYModel>(criteria).ConfigureAwait(false);
        }

        public async Task<EOQYModel> GetEOYByIdAsync(int eoyId)
        {
            var criteria = new GetEOYByIdCriteria
            {
                EOYId = eoyId
            };

            return await baseService.GetDataEntityAsync<EOQYModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> CreateEOYAsync(CreateEOYModel eoy)
        {
            var criteria = new CreateEOYCriteria
            {
                Year = eoy.Year,
                StartDateForNominator = eoy.StartDateForNominator,
                EndDateForNominator = eoy.EndDateForNominator,
                RemindDateForNominator = eoy.RemindDateForNominator,
                EndDateForCommittee = eoy.EndDateForCommittee,
                Description = eoy.Description,
                CreatedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditEOYAsync(EOQYModel eoy)
        {
            var criteria = new EditEOYCriteria
            {
                EOYId = eoy.EOQYId,
                Year = eoy.Year,
                StartDateForNominator = eoy.StartDateForNominator,
                EndDateForNominator = eoy.EndDateForNominator,
                RemindDateForNominator = eoy.RemindDateForNominator,
                EndDateForCommittee = eoy.EndDateForCommittee,
                Description = eoy.Description,
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> DeleteEOYAsync(int eoyId)
        {
            var criteria = new DeleteEOYCriteria
            {
                EOYId = eoyId,
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}