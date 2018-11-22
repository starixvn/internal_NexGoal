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

        public async Task<CommandResult> CreateAsync(CreateEOQModel eoq)
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

        public async Task<CommandResult> EditAsync(EditEOQModel eoq)
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
                Description = eoq.Description,
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<NominatorSettingModel>> GetNominatorSettingAsync(
            int eoqId,
            string keyword = null,
            string groupIds = null,
            int? roleId = null,
            bool isShowAll = false)
        {
            var criteria = new GetNominatorSettingCriteria
            {
                EOQId = eoqId,
                Keyword = keyword,
                GroupIds = groupIds,
                RoleId = roleId,
                IsShowAll = isShowAll
            };

            return await baseService.GetDataAsync<NominatorSettingModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditNominatorSettingAsync(EditNominatorSettingModel setting)
        {
            var criteria = new EditNominatorSettingCriteria
            {
                EOQId = setting.EOQYId,
                EmployeeId = setting.EmployeeId,
                RoleIds = setting.RoleIds,
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}