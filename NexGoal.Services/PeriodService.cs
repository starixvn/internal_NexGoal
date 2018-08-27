namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Periods;
    using NexGoal.Models.Period;

    public class PeriodService : IPeriodService
    {
        private readonly IBaseService baseService;
        private readonly DynamicRepository dynamicRepository;

        public PeriodService(IBaseService baseService)
        {
            this.baseService = baseService;
            this.dynamicRepository = new DynamicRepository();
        }

        public async Task<IEnumerable<PeriodModel>> GetPeriods(int? targetTypeId, bool? isActive)
        {
            return await baseService.GetDataAsync<PeriodModel>(new GetPeriodsCriteria { TargetTypeId = targetTypeId, IsActive = isActive }).ConfigureAwait(false);
        }

        public async Task<PeriodModel> GetPeriod(int periodId)
        {
            return await baseService.GetDataEntityAsync<PeriodModel>(new GetPeriodCriteria { PeriodId = periodId }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PeriodProcessSettingModel>> GetPeriodProcessSetting(int periodId)
        {
            return await baseService.GetDataAsync<PeriodProcessSettingModel>(new GetPeriodProcessSettingCriteria { PeriodId = periodId }).ConfigureAwait(false);
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

        public async Task<CommandResult> Edit(EditPeriodModel model)
        {
            var criteria = new EditPeriodCriteria
            {
                PeriodId = model.PeriodId,
                Name = model.Name,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                DisciplineStartDate = model.DisciplineStartDate,
                DisciplineEndDate = model.DisciplineEndDate,
                FocusStartDate = model.FocusStartDate,
                Description = model.Description,
                IsActive = model.IsActive,
                ModifiedBy = model.ModifiedBy
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditProcessSetting(int periodId, IEnumerable<EditProcessSettingModel> settings, int modifiedBy)
        {
            var criteria = new EditProcessSettingCriteria
            {
                PeriodId = periodId,
                Settings = settings.ToXML().ToString(),
                ModifiedBy = modifiedBy
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PARecordModel>> GetEmployeeSettings(int periodId, int? groupId, bool? isLocal, bool? isEvaluated, int? statusId)
        {
            var criteria = new GetEmployeeSettingsCriteria
            {
                PeriodId = periodId,
                GroupId = groupId,
                IsLocal = isLocal,
                IsEvaluated = isEvaluated,
                StatusId = statusId
            };

            var result = await dynamicRepository.FetchMultipleAsync<PARecordModel, PAProcessModel>(criteria).ConfigureAwait(false);

            var paRecords = result.Item1;
            var paProcesses = result.Item2;

            foreach (var paRecord in paRecords)
            {
                paRecord.Processes = paProcesses.Where(x => x.PARecordId == paRecord.PARecordId);
            }

            return paRecords;
        }

        public async Task<CommandResult> CreateEmployeeSettings(int periodId, string groupIds, int? employeeId)
        {
            var criteria = new CreateEmployeeSettingsCriteria
            {
                PeriodId = periodId,
                GroupIds = groupIds,
                EmployeeId = employeeId
            };

            var result = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return result;
        }
    }
}