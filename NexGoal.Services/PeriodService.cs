namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Periods;
    using NexGoal.Models.Period;

    public class PeriodService : IPeriodService
    {
        private readonly IBaseService baseService;

        public PeriodService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<PeriodModel>> GetPeriods(int? targetTypeId, bool? isActive)
        {
            return await baseService.GetDataAsync<PeriodModel>(new GetPeriodsCriteria { TargetTypeId = targetTypeId, IsActive = isActive }).ConfigureAwait(false);
        }

        public async Task<CommandResult> Create(CreatePeriodModel model)
        {
            var criteria = new CreatePeriodCriteria
            {
                Name = model.Name,
                TargetTypeId = model.TargetTypeId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                DisciplineStartDate = model.DisciplineStartDate,
                DisciplineEndDate = model.DisciplineEndDate,
                FocusStartDate = model.FocusStartDate,
                Description = model.Description,
                IsActive = model.IsActive,
                CreatedBy = model.CreatedBy
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditStatus(int periodId, bool isActive, int modifiedBy)
        {
            var criteria = new UpdatePeriodStatusCriteria
            {
                PeriodId = periodId,
                IsActive = isActive,
                ModifiedBy = modifiedBy
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> Delete(int periodId, int modifiedBy)
        {
            var criteria = new DeletePeriodCriteria
            {
                PeriodId = periodId,
                ModifiedBy = modifiedBy
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}