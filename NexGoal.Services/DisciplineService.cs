namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Discipline;
    using NexGoal.Models.Discipline;

    public class DisciplineService : IDisciplineService
    {
        private readonly IBaseService baseService;

        public DisciplineService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<DisciplineModel>> GetDisciplines(DateTime from, DateTime to, bool? isActive, string groupIds, int? employeeId)
        {
            var criteria = new GetDisciplinesCriteria
            {
                FromDate = from,
                ToDate = to,
                IsActive = isActive,
                GroupIds = groupIds,
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<DisciplineModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditStatus(int disciplineId, bool isActive, int modifiedBy)
        {
            var criteria = new UpdateDisciplineStatusCriteria
            {
                DisciplineId = disciplineId,
                IsActive = isActive,
                ModifiedBy = modifiedBy
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> Create(AddDisciplineModel model)
        {
            var criteria = new AddDisciplineCriteria
            {
                EmployeeId = model.EmployeeId,
                EffectiveDate = model.EffectiveDate,
                Description = model.Content,
                CreatedBy = model.CreatedBy
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> Edit(EditDisciplineModel model)
        {
            var criteria = new EditDisciplineCriteria
            {
                DisciplineId = model.DisciplineId,
                EffectiveDate = model.EffectiveDate,
                Description = model.Content,
                ModifiedBy = model.ModifiedBy
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}