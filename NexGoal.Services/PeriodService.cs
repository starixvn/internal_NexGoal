namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Periods;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Period;

    public class PeriodService : IPeriodService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly ICommonService commonService;

        public PeriodService(
            IBaseService baseService,
            ISessionService sessionService,
            ICommonService commonService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<PeriodModel>> GetPeriodAsync(bool? isActive = null)
        {
            var criteria = new GetPeriodsCriteria { IsActive = isActive };

            return await baseService.GetDataAsync<PeriodModel>(criteria).ConfigureAwait(false);
        }

        public async Task<PeriodModel> GetByIdAsync(int periodId)
        {
            var criteria = new GetPeriodCriteria { PeriodId = periodId };

            return await baseService.GetDataEntityAsync<PeriodModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PeriodProcessSettingModel>> GetPeriodProcessSettingAsync(int periodId)
        {
            var criteria = new GetPeriodProcessSettingCriteria { PeriodId = periodId };

            return await baseService.GetDataAsync<PeriodProcessSettingModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> CreateAsync(CreatePeriodModel model)
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
                FocusEndDate = model.FocusEndDate,
                Description = model.Description,
                IsActive = model.IsActive,
                CreatedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditStatusAsync(int periodId, bool isActive)
        {
            var criteria = new UpdatePeriodStatusCriteria
            {
                PeriodId = periodId,
                IsActive = isActive,
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> DeleteAsync(int periodId)
        {
            var criteria = new DeletePeriodCriteria
            {
                PeriodId = periodId,
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditAsync(EditPeriodModel model)
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
                FocusEndDate = model.FocusEndDate,
                Description = model.Description,
                IsActive = model.IsActive,
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditProcessSettingAsync(int periodId, IEnumerable<EditProcessSettingModel> settings)
        {
            var criteria = new EditProcessSettingCriteria
            {
                PeriodId = periodId,
                Settings = settings.ToXML().ToString(),
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PARecordModel>> GetEmployeeSettingAsync(int periodId, int? groupId = null, bool? isEvaluated = null, int? statusId = null)
        {
            var criteria = new GetEmployeeSettingsCriteria
            {
                PeriodId = periodId,
                GroupId = groupId,
                IsEvaluated = isEvaluated,
                StatusId = statusId
            };

            var result = await baseService.FetchMultipleAsync<PARecordModel, PAProcessModel>(criteria).ConfigureAwait(false);
            IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);

            IEnumerable<PARecordModel> paRecords = result.Item1;
            IEnumerable<PAProcessModel> paProcesses = result.Item2;

            foreach (PAProcessModel process in paProcesses)
            {
                process.Evaluator = employees.FirstOrDefault(x => x.EmployeeId == process.EvaluatorId);
            }

            foreach (PARecordModel paRecord in paRecords)
            {
                paRecord.Employee = employees.FirstOrDefault(x => x.EmployeeId == paRecord.EmployeeId);
                paRecord.Processes = paProcesses.Where(x => x.PARecordId == paRecord.PARecordId);
            }

            return paRecords;
        }

        public async Task<CommandResult> CreateEmployeeSettingAsync(int periodId, string groupIds, int? employeeId = null)
        {
            var criteria = new CreateEmployeeSettingsCriteria
            {
                PeriodId = periodId,
                GroupIds = groupIds,
                EmployeeId = employeeId,
                CreatedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditEmployeeSettingAsync(int periodId, int[] paRecordIds, IEnumerable<EditEmployeeSettingModel> processes)
        {
            var criteria = new EditEmployeeSettingsCriteria
            {
                PeriodId = periodId,
                PARecordIds = string.Join(',', paRecordIds),
                Settings = processes.ToXML().ToString(),
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditEmployeeSettingStatusAsync(int paRecordId, int statusId)
        {
            var criteria = new EditEmployeeSettingStatusCriteria
            {
                PARecordId = paRecordId,
                StatusId = statusId,
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<PARecordModel> GetEmployeeSettingByIdAsync(int paRecordId)
        {
            var criteria = new GetEmployeeSettingByIdCriteria
            {
                PARecordId = paRecordId
            };

            var result = await baseService.FetchMultipleAsync<PARecordModel, PAProcessModel>(criteria).ConfigureAwait(false);

            PARecordModel paRecord = result.Item1.FirstOrDefault();
            if (paRecord != null)
            {
                IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);
                paRecord.Employee = employees.FirstOrDefault(x => x.EmployeeId == paRecord.EmployeeId);

                foreach (PAProcessModel process in result.Item2)
                {
                    process.Evaluator = employees.FirstOrDefault(x => x.EmployeeId == process.EvaluatorId);
                }

                paRecord.Processes = result.Item2;
            }

            return paRecord;
        }

        public async Task<CommandResult> GeneratePAAsync(int periodId, string paRecordIds, int createdBy)
        {
            var criteria = new GeneratePACriteria
            {
                PeriodId = periodId,
                PARecordIds = paRecordIds,
                CreatedBy = createdBy,
                Override = true
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<PeriodBaseModel> GetCurrentPeriod()
        {
            var criteria = new GetCurrentPerformancePeriodCriteria();

            return await baseService.GetDataEntityAsync<PeriodBaseModel>(criteria).ConfigureAwait(false);
        }
    }
}