namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Periods;
    using NexGoal.Models.Period;
    using NexGoal.Models.UserManagement;

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

        public async Task<IEnumerable<PeriodModel>> GetPeriods(bool? isActive)
        {
            var criteria = new GetPeriodsCriteria { IsActive = isActive };

            return await baseService.GetDataAsync<PeriodModel>(criteria).ConfigureAwait(false);
        }

        public async Task<PeriodModel> GetPeriod(int periodId)
        {
            var criteria = new GetPeriodCriteria { PeriodId = periodId };

            return await baseService.GetDataEntityAsync<PeriodModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PeriodProcessSettingModel>> GetPeriodProcessSetting(int periodId)
        {
            var criteria = new GetPeriodProcessSettingCriteria { PeriodId = periodId };

            return await baseService.GetDataAsync<PeriodProcessSettingModel>(criteria).ConfigureAwait(false);
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
                FocusEndDate = model.FocusEndDate,
                Description = model.Description,
                IsActive = model.IsActive,
                CreatedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditStatus(int periodId, bool isActive)
        {
            var criteria = new UpdatePeriodStatusCriteria
            {
                PeriodId = periodId,
                IsActive = isActive,
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> Delete(int periodId)
        {
            var criteria = new DeletePeriodCriteria
            {
                PeriodId = periodId,
                ModifiedBy = sessionService.EmployeeId
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
                FocusEndDate = model.FocusEndDate,
                Description = model.Description,
                IsActive = model.IsActive,
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditProcessSetting(int periodId, IEnumerable<EditProcessSettingModel> settings)
        {
            var criteria = new EditProcessSettingCriteria
            {
                PeriodId = periodId,
                Settings = settings.ToXML().ToString(),
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PARecordModel>> GetEmployeeSettings(int periodId, int? groupId, bool? isEvaluated, int? statusId)
        {
            var criteria = new GetEmployeeSettingsCriteria
            {
                PeriodId = periodId,
                GroupId = groupId,
                IsEvaluated = isEvaluated,
                StatusId = statusId
            };

            var result = await baseService.FetchMultipleAsync<PARecordModel, PAProcessModel>(criteria).ConfigureAwait(false);
            IEnumerable<UserModel> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);

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

        public async Task<CommandResult> CreateEmployeeSettings(int periodId, string groupIds, int? employeeId)
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

        public async Task<CommandResult> EditEmployeeSettings(int periodId, int[] paRecordIds, IEnumerable<EditEmployeeSettingModel> processes)
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

        public async Task<CommandResult> EditEmployeeSettingStatus(int paRecordId, int statusId)
        {
            var criteria = new EditEmployeeSettingStatusCriteria
            {
                PARecordId = paRecordId,
                StatusId = statusId,
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<PARecordModel> GetEmployeeSetting(int paRecordId)
        {
            var criteria = new GetEmployeeSettingCriteria
            {
                PARecordId = paRecordId
            };

            var result = await baseService.FetchMultipleAsync<PARecordModel, PAProcessModel>(criteria).ConfigureAwait(false);

            PARecordModel paRecord = result.Item1.FirstOrDefault();
            if (paRecord != null)
            {
                IEnumerable<UserModel> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);
                paRecord.Employee = employees.FirstOrDefault(x => x.EmployeeId == paRecord.EmployeeId);

                foreach (PAProcessModel process in result.Item2)
                {
                    process.Evaluator = employees.FirstOrDefault(x => x.EmployeeId == process.EvaluatorId);
                }

                paRecord.Processes = result.Item2;
            }

            return paRecord;
        }
    }
}