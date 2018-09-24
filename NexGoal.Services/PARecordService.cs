namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PARecord;
    using NexGoal.Models;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.Period;
    using NexGoal.Models.UserManagement;

    public class PARecordService : IPARecordService
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;

        public PARecordService(
            IBaseService baseService,
            ICryptoService cryptoService,
            ICommonService commonService)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<PeriodModel>> GetPeriodByActorAsync(int actorId)
        {
            var criteria = new GetPeriodByActorCriteria
            {
                ActorId = actorId
            };

            return await baseService.GetDataAsync<PeriodModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<UserGroupModel>> GetGroupByActorAsync(int actorId, int periodId)
        {
            var criteria = new GetGroupByActorCriteria
            {
                ActorId = actorId,
                PeriodId = periodId
            };

            return await baseService.GetDataAsync<UserGroupModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PARecordModel>> GetPARecordByActorAsync(int actorId, int periodId)
        {
            var criteria = new GetPARecordByActorCriteria
            {
                ActorId = actorId,
                PeriodId = periodId
            };
            var tuple = await baseService.FetchMultipleAsync<PARecordModel, PAProcessModel, PerformanceRecordModel>(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
            IEnumerable<UserModel> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);

            MappingData(tuple.Item1, tuple.Item2, tuple.Item3, employees, performanceGrades);

            return tuple.Item1;
        }

        private void MappingData(
            IEnumerable<PARecordModel> paRecords,
            IEnumerable<PAProcessModel> processes,
            IEnumerable<PerformanceRecordModel> performanceRecords,
            IEnumerable<UserModel> employees,
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
    }
}