namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Periods;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Period;

    public class PeriodService : IPeriodService
    {
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;
        private readonly ICryptoService cryptoService;

        public PeriodService(
            IBaseService baseService,
            ICommonService commonService,
            ICryptoService cryptoService)
        {
            this.baseService = baseService;
            this.commonService = commonService;
            this.cryptoService = cryptoService;
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

        public async Task<CommandResult> CreateAsync(int actorId, CreatePeriodModel model)
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
                CreatedBy = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditStatusAsync(int actorId, int periodId, bool isActive)
        {
            var criteria = new UpdatePeriodStatusCriteria
            {
                PeriodId = periodId,
                IsActive = isActive,
                ModifiedBy = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> DeleteAsync(int actorId, int periodId)
        {
            var criteria = new DeletePeriodCriteria
            {
                PeriodId = periodId,
                ModifiedBy = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditAsync(int actorId, EditPeriodModel model)
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
                ModifiedBy = actorId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            if (commandResult.IsSuccessful && model.EndDate.HasValue && model.EndDate.Value.Date < DateTime.Now.Date)
            {
                commandResult = await EditFinalPAGradeAsync(actorId, criteria.PeriodId).ConfigureAwait(false);
            }

            return commandResult;
        }

        public async Task<CommandResult> EditProcessSettingAsync(int actorId, int periodId, IEnumerable<EditProcessSettingModel> settings)
        {
            var criteria = new EditProcessSettingCriteria
            {
                PeriodId = periodId,
                Settings = settings.ToXML().ToString(),
                ModifiedBy = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PARecordModel>> GetEmployeeSettingAsync(
            int periodId,
            int? groupId = null,
            bool? isEvaluated = null,
            int? statusId = null)
        {
            var criteria = new GetEmployeeSettingsCriteria
            {
                PeriodId = periodId,
                GroupId = groupId,
                IsEvaluated = isEvaluated,
                StatusId = statusId
            };

            var result = await baseService.FetchMultipleAsync<PARecordModel, PAProcessModel>(criteria).ConfigureAwait(false);
            IEnumerable<Employee> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);

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

        public async Task<CommandResult> CreateEmployeeSettingAsync(int actorId, int periodId, string groupIds, int? employeeId = null)
        {
            var criteria = new CreateEmployeeSettingsCriteria
            {
                PeriodId = periodId,
                GroupIds = groupIds,
                EmployeeId = employeeId,
                CreatedBy = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditEmployeeSettingAsync(
            int actorId,
            int periodId,
            int[] paRecordIds,
            IEnumerable<EditEmployeeSettingModel> processes)
        {
            var criteria = new EditEmployeeSettingsCriteria
            {
                PeriodId = periodId,
                PARecordIds = string.Join(',', paRecordIds),
                Settings = processes.ToXML().ToString(),
                ModifiedBy = actorId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            if (commandResult.IsSuccessful)
            {
                commandResult = await EditFinalPAGradeAsync(actorId, criteria.PeriodId, criteria.PARecordIds).ConfigureAwait(false);
            }

            return commandResult;
        }

        public async Task<CommandResult> EditEmployeeSettingStatusAsync(int actorId, int paRecordId, bool isExcluded)
        {
            var criteria = new EditEmployeeSettingStatusCriteria
            {
                PARecordId = paRecordId,
                IsExcluded = isExcluded,
                ModifiedBy = actorId
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
                IEnumerable<Employee> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);
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

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            if (result.IsSuccessful)
            {
                result = await EditPAGradeAsync(paRecordIds).ConfigureAwait(false);
            }

            return result;
        }

        private async Task<IEnumerable<EditPAGradeDto>> GetDataToEditPAGradeAsync(string paRecordIds)
        {
            var criteria = new GetPARecordsToUpdateGradeCriteria
            {
                PARecordIds = paRecordIds
            };
            var records = await baseService.GetDataAsync<EditPAGradeDto>(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGrade> grades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            foreach (EditPAGradeDto record in records)
            {
                record.GradeId = cryptoService.Decrypt(record.GradeId);
                int.TryParse(record.GradeId, out int gradeId);
                gradeId += record.ToeicPolicy;

                if (!grades.Any(x => x.GradeId == gradeId))
                {
                    gradeId = gradeId > grades.Max(x => x.GradeId) ? grades.Max(x => x.GradeId) : 0;
                }

                record.GradeId = gradeId == 0 ? null : cryptoService.Encrypt(gradeId.ToString());
                record.OriginalGradeId = record.GradeId;
            }

            return records;
        }

        private async Task<CommandResult> EditPAGradeAsync(string paRecordIds)
        {
            var commandResult = new CommandResult { Code = 0 };
            IEnumerable<EditPAGradeDto> records = await GetDataToEditPAGradeAsync(paRecordIds).ConfigureAwait(false);

            if (records.Any())
            {
                var criteria = new EditPARecordGradeCriteria
                {
                    Value = records.ToXML().ToString()
                };

                commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            }

            return commandResult;
        }

        private async Task<IEnumerable<EditPAGradeDto>> GetDataToEditFinalPAGradeAsync(int periodId, string paRecordIds = null)
        {
            var criteria = new GetDataToEditFinalPAGradeCriteria
            {
                PeriodId = periodId,
                PARecordIds = paRecordIds
            };
            var records = await baseService.GetDataAsync<EditPAGradeDto>(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGrade> grades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
            int minGradeId = grades.Min(x => x.GradeId);
            int maxGradeId = grades.Max(x => x.GradeId);

            foreach (EditPAGradeDto record in records)
            {
                record.GradeId = cryptoService.Decrypt(record.GradeId);
                int.TryParse(record.GradeId, out int gradeId);

                record.OriginalGradeId = cryptoService.Decrypt(record.OriginalGradeId);
                int.TryParse(record.OriginalGradeId, out int originalGradeId);

                if (!grades.Any(x => x.GradeId == gradeId))
                {
                    gradeId = gradeId > maxGradeId ? maxGradeId : minGradeId;
                }

                if (!grades.Any(x => x.GradeId == originalGradeId))
                {
                    originalGradeId = originalGradeId > maxGradeId ? maxGradeId : minGradeId;
                }

                record.GradeId = cryptoService.Encrypt(gradeId.ToString());
                record.OriginalGradeId = cryptoService.Encrypt(originalGradeId.ToString());
            }

            return records;
        }

        private async Task<CommandResult> EditFinalPAGradeAsync(int actorId, int periodId, string paRecordIds = null)
        {
            var commandResult = new CommandResult { Code = 0 };
            IEnumerable<EditPAGradeDto> records = await GetDataToEditFinalPAGradeAsync(periodId, paRecordIds).ConfigureAwait(false);

            if (records.Any())
            {
                var criteria = new EditFinalPAGradeCriteria
                {
                    ActorId = actorId,
                    Value = records.ToXML().ToString()
                };

                commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            }

            return commandResult;
        }
    }
}