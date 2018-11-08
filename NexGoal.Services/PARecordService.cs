namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PARecord;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.PerformanceRecord;

    public class PARecordService : IPARecordService
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;
        private readonly IAvatarService avatarService;

        public PARecordService(
            IBaseService baseService,
            ICryptoService cryptoService,
            ICommonService commonService,
            IAvatarService avatarService)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
            this.avatarService = avatarService;
        }

        public async Task<IEnumerable<PeriodBaseModel>> GetPeriodByActorAsync(int actorId)
        {
            var criteria = new GetPeriodByActorCriteria
            {
                ActorId = actorId
            };

            return await baseService.GetDataAsync<PeriodBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupBaseModel>> GetGroupByActorAsync(int actorId, int periodId)
        {
            var criteria = new GetGroupByActorCriteria
            {
                ActorId = actorId,
                PeriodId = periodId
            };

            return await baseService.GetDataAsync<GroupBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PARecordModel>> GetPARecordByActorAsync(GetPARecordParam param)
        {
            var criteria = new GetPARecordByActorCriteria
            {
                ActorId = param.ActorId,
                PeriodId = param.PeriodId,
                GroupIds = param.GroupIds
            };
            var tuple = await baseService.FetchMultipleAsync<PARecordModel, PAProcessModel, PerformanceRecordModel>(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
            IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);

            MappingData(tuple.Item1, tuple.Item2, tuple.Item3, employees, performanceGrades);

            return tuple.Item1;
        }

        public async Task<PARecordModel> GetPARecordByIdAsync(int paRecordId, int actorId)
        {
            var criteria = new GetPARecordByIdCriteria
            {
                PARecordId = paRecordId,
                ActorId = actorId
            };

            var tuple = await baseService.FetchMultipleAsync<PARecordModel, PAProcessModel, PerformanceRecordModel>(criteria).ConfigureAwait(false);
            IEnumerable<ProjectModel> projects = await commonService.GetProjectsAsync().ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
            IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);

            PARecordModel record = tuple.Item1.FirstOrDefault();

            if (record != null)
            {
                MappingData(record, tuple.Item2, tuple.Item3, employees, performanceGrades, projects);
            }

            return record;
        }

        public async Task<CommandResult> EditPARecordByReviewerAsync(int actorId, EditPARecordByReviewerModel paRecord)
        {
            var criteria = new EditPARecordByReviewerCriteria
            {
                PARecordId = paRecord.PARecordId,
                GradeId = cryptoService.Encrypt(paRecord.GradeId),
                Comment = paRecord.Comment,
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        private void MappingData(
            IEnumerable<PARecordModel> paRecords,
            IEnumerable<PAProcessModel> processes,
            IEnumerable<PerformanceRecordModel> performanceRecords,
            IEnumerable<EmployeeBaseModel> employees,
            IEnumerable<PerformanceGradeModel> performanceGrades)
        {
            foreach (PerformanceRecordModel record in performanceRecords)
            {
                record.GradeId = cryptoService.Decrypt(record.GradeId);
                int.TryParse(record.GradeId, out int gradeId);
                record.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
            }

            foreach (PAProcessModel process in processes)
            {
                process.Evaluator = employees.FirstOrDefault(x => x.EmployeeId == process.EvaluatorId);
                process.PerformanceRecord = performanceRecords.FirstOrDefault(x => x.PerformanceRecordId == process.PerformanceRecordId);
            }

            foreach (PARecordModel record in paRecords)
            {
                record.Employee = employees.FirstOrDefault(x => x.EmployeeId == record.EmployeeId);

                record.GradeId = cryptoService.Decrypt(record.GradeId);
                int.TryParse(record.GradeId, out int gradeId);
                record.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

                record.Processes = processes.Where(x => x.PARecordId == record.PARecordId);
            }
        }

        private void MappingData(
            PARecordModel paRecord,
            IEnumerable<PAProcessModel> processes,
            IEnumerable<PerformanceRecordModel> performanceRecords,
            IEnumerable<EmployeeBaseModel> employees,
            IEnumerable<PerformanceGradeModel> performanceGrades,
            IEnumerable<ProjectModel> projects)
        {
            int gradeId;
            foreach (PerformanceRecordModel record in performanceRecords)
            {
                string[] projectIds = !string.IsNullOrWhiteSpace(record.ProjectIds) ? record.ProjectIds.Split(",") : new string[] { };
                record.Projects = projects.Where(x => projectIds.Contains(x.ProjectId.ToString()));

                record.GradeId = cryptoService.Decrypt(record.GradeId);
                int.TryParse(record.GradeId, out gradeId);
                record.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
            }

            foreach (PAProcessModel process in processes)
            {
                process.Evaluator = employees.FirstOrDefault(x => x.EmployeeId == process.EvaluatorId);
                if (process.Evaluator != null)
                {
                    process.Evaluator.Avatar = avatarService.GetFilePath(process.Evaluator.Code);
                }

                process.PerformanceRecord = performanceRecords.FirstOrDefault(x => x.PerformanceRecordId == process.PerformanceRecordId);
            }

            paRecord.Employee = employees.FirstOrDefault(x => x.EmployeeId == paRecord.EmployeeId);

            paRecord.GradeId = cryptoService.Decrypt(paRecord.GradeId);
            int.TryParse(paRecord.GradeId, out gradeId);
            paRecord.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

            paRecord.Processes = processes.Where(x => x.PARecordId == paRecord.PARecordId);
        }
    }
}