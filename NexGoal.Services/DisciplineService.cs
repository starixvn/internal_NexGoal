namespace NexGoal.Services
{
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

        public async Task<IEnumerable<DisciplineModel>> GetDisciplineAsync(GetDisciplineParam param)
        {
            var criteria = new GetDisciplineCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                IsActive = param.IsActive,
                GroupId = param.GroupId,
                EmployeeId = param.EmployeeId
            };

            return await baseService.GetDataAsync<DisciplineModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditStatusAsync(int disciplineId, bool isActive, int modifiedBy)
        {
            var criteria = new UpdateDisciplineStatusCriteria
            {
                DisciplineId = disciplineId,
                IsActive = isActive,
                ModifiedBy = modifiedBy
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> CreateAsync(AddDisciplineModel model)
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

        public async Task<CommandResult> EditAsync(EditDisciplineModel model)
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

        public async Task<IEnumerable<DisciplineModel>> GetCurrentPeriodDisciplineAsync(int employeeId)
        {
            var criteria = new GetCurrentPeriodDisciplineCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<DisciplineModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DisciplineModel>> GetEmployeeDisciplineByPeriodAsync(int employeeId, int periodId)
        {
            var criteria = new GetEmployeeDisciplineByPeriodCriteria
            {
                EmployeeId = employeeId,
                PeriodId = periodId
            };

            return await baseService.GetDataAsync<DisciplineModel>(criteria).ConfigureAwait(false);
        }
    }
}