namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Discipline;
    using NexGoal.Models.Discipline;
    using NexGoal.Repositories;

    public class DisciplineService : IDisciplineService
    {
        private readonly IBaseService baseService;
        private readonly IDisciplineRepository disciplineRepository;

        public DisciplineService(
            IBaseService baseService,
            IDisciplineRepository disciplineRepository)
        {
            this.baseService = baseService;
            this.disciplineRepository = disciplineRepository;
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

            return await disciplineRepository.GetDisciplineAsync(criteria).ConfigureAwait(false);
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

        public async Task<IEnumerable<DisciplineModel>> GetDisciplineByEmployeeIdAsync(int employeeId, int? periodId = null)
        {
            var criteria = new GetDisciplineByEmployeeIdCriteria
            {
                EmployeeId = employeeId,
                PeriodId = periodId
            };

            return await baseService.GetDataAsync<DisciplineModel>(criteria).ConfigureAwait(false);
        }
    }
}