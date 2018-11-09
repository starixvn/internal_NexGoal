namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceRecord;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord;

    public class PerformanceRecordService : IPerformanceRecordService
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;

        public PerformanceRecordService(
            IBaseService baseService,
            ICryptoService cryptoService,
            ICommonService commonService)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<PerformanceRecordModel>> GetPerformanceRecordByEmployeeAsync(int employeeId)
        {
            var criteria = new GetPerformanceRecordByEmployeeCriteria
            {
                EmployeeId = employeeId
            };
            var performanceRecords = await baseService.GetDataAsync<PerformanceRecordModel>(criteria).ConfigureAwait(false);
            IEnumerable<ProjectModel> projects = await commonService.GetProjectsAsync().ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
            IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);

            foreach (PerformanceRecordModel performanceRecord in performanceRecords)
            {
                MappingDataPerformanceRecord(performanceRecord, projects, performanceGrades, employees);
            }

            return performanceRecords;
        }

        public async Task<Tuple<IEnumerable<PerformanceRecordModel>, int>> GetPerformanceTimelineAsync(GetPerformanceTimelineParam param)
        {
            var criteria = new GetPerformanceTimelineCriteria
            {
                EmployeeId = param.EmployeeId,
                PageIndex = param.PageIndex,
                PageSize = param.PageSize
            };
            var performanceRecords = await baseService.GetDataAsync<PerformanceRecordModel>(criteria).ConfigureAwait(false);

            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
            IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync().ConfigureAwait(false);

            foreach (PerformanceRecordModel performanceRecord in performanceRecords)
            {
                MappingDataPerformanceTimeline(performanceRecord, performanceGrades, employees);
            }

            return new Tuple<IEnumerable<PerformanceRecordModel>, int>(performanceRecords, criteria.TotalItems);
        }

        public async Task<IEnumerable<PerformanceRecordModel>> GetMemberPerformanceAsync(int actorId, int? employeeId = null)
        {
            var criteria = new GetMemberPerformanceCriteria
            {
                ActorId = actorId,
                EmployeeId = employeeId
            };
            var performanceRecords = await baseService.GetDataAsync<PerformanceRecordModel>(criteria).ConfigureAwait(false);
            IEnumerable<ProjectModel> projects = await commonService.GetProjectsAsync().ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
            IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);

            foreach (PerformanceRecordModel performanceRecord in performanceRecords)
            {
                MappingDataPerformanceRecord(performanceRecord, projects, performanceGrades, employees);
            }

            return performanceRecords;
        }

        public async Task<IEnumerable<MemberPerformanceTimelineModel>> GetMemberPerformanceTimelineAsyn(GetMemberPerformanceTimelineParam param)
        {
            var criteria = new GetMemberPerformanceTimelineCriteria
            {
                ActorId = param.ActorId,
                PeriodId = param.PeriodId,
                EmployeeId = param.EmployeeId,
                PageIndex = param.PageIndex,
                PageSize = param.PageSize
            };

            var tuple = await baseService.FetchMultipleAsync<MemberPerformanceTimelineModel, PerformanceRecordModel>(criteria).ConfigureAwait(false);

            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
            IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync().ConfigureAwait(false);

            MappingDataMemberPerformanceTime(tuple.Item1, tuple.Item2, performanceGrades, employees);

            return tuple.Item1;
        }

        public async Task<PerformanceRecordModel> GetPerformanceRecordByIdAsync(
            int performanceRecordId,
            int creatorId)
        {
            var criteria = new GetPerformanceRecordByIdCriteria
            {
                PerformanceRecordId = performanceRecordId,
                CreatorId = creatorId
            };
            var performanceRecord = await baseService.GetDataEntityAsync<PerformanceRecordModel>(criteria).ConfigureAwait(false);

            if (performanceRecord != null)
            {
                IEnumerable<ProjectModel> projects = await commonService.GetProjectsAsync().ConfigureAwait(false);
                IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
                IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);

                MappingDataPerformanceRecord(performanceRecord, projects, performanceGrades, employees);
            }

            return performanceRecord;
        }

        public async Task<CommandResult> CreatePerformanceRecordAsync(CreatePerformanceRecordModel performanceRecord)
        {
            var criteria = new CreatePerformanceRecordCriteria
            {
                EmployeeIds = performanceRecord.EmployeeIds,
                CreatorId = performanceRecord.CreatorId,
                GradeId = cryptoService.Encrypt(performanceRecord.GradeId),
                Comment = performanceRecord.Comment,
                ProjectIds = performanceRecord.ProjectIds,
                StatusId = performanceRecord.StatusId,
                TypeId = performanceRecord.TypeId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<CommandResult> EditPerformanceRecordAsync(EditPerformanceRecordModel performanceRecord)
        {
            var criteria = new EditPerformanceRecordCriteria
            {
                PerformanceRecordId = performanceRecord.PerformanceRecordId,
                EmployeeIds = performanceRecord.EmployeeIds,
                CreatorId = performanceRecord.CreatorId,
                GradeId = cryptoService.Encrypt(performanceRecord.GradeId),
                Comment = performanceRecord.Comment,
                ProjectIds = performanceRecord.ProjectIds,
                StatusId = performanceRecord.StatusId,
                TypeId = performanceRecord.TypeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        private void MappingDataPerformanceRecord(
            PerformanceRecordModel performanceRecord,
            IEnumerable<ProjectModel> projects,
            IEnumerable<PerformanceGradeModel> performanceGrades,
            IEnumerable<EmployeeBaseModel> employees)
        {
            string[] projectIds = !string.IsNullOrWhiteSpace(performanceRecord.ProjectIds) ? performanceRecord.ProjectIds.Split(",") : new string[] { };
            performanceRecord.Projects = projects.Where(x => projectIds.Contains(x.ProjectId.ToString()));

            string[] employeeIds = !string.IsNullOrWhiteSpace(performanceRecord.EmployeeIds) ? performanceRecord.EmployeeIds.Split(",") : new string[] { };
            performanceRecord.Employees = employees.Where(x => employeeIds.Contains(x.EmployeeId.ToString()));

            performanceRecord.GradeId = cryptoService.Decrypt(performanceRecord.GradeId);
            int.TryParse(performanceRecord.GradeId, out int gradeId);
            performanceRecord.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

            performanceRecord.Creator = performanceRecord.CreatorId == Constants.NexGoalSystemId ?
                NexGoalSystemInfo.NexGoal : employees.FirstOrDefault(x => x.EmployeeId == performanceRecord.CreatorId);
        }

        private void MappingDataPerformanceTimeline(
            PerformanceRecordModel performanceRecord,
            IEnumerable<PerformanceGradeModel> performanceGrades,
            IEnumerable<EmployeeBaseModel> employees)
        {
            performanceRecord.GradeId = cryptoService.Decrypt(performanceRecord.GradeId);
            int.TryParse(performanceRecord.GradeId, out int gradeId);
            performanceRecord.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

            performanceRecord.Creator = performanceRecord.CreatorId == Constants.NexGoalSystemId ?
                NexGoalSystemInfo.NexGoal : employees.FirstOrDefault(x => x.EmployeeId == performanceRecord.CreatorId);
        }

        private void MappingDataMemberPerformanceTime(
            IEnumerable<MemberPerformanceTimelineModel> memberPerformances,
            IEnumerable<PerformanceRecordModel> performanceRecords,
            IEnumerable<PerformanceGradeModel> performanceGrades,
            IEnumerable<EmployeeBaseModel> employees)
        {
            foreach (PerformanceRecordModel performanceRecord in performanceRecords)
            {
                MappingDataPerformanceTimeline(performanceRecord, performanceGrades, employees);
            }

            foreach (MemberPerformanceTimelineModel memberPerformance in memberPerformances)
            {
                memberPerformance.GradeId = cryptoService.Decrypt(memberPerformance.GradeId);
                int.TryParse(memberPerformance.GradeId, out int gradeId);
                memberPerformance.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

                memberPerformance.PerformanceRecords = performanceRecords.Where(x => x.EmployeeIds == memberPerformance.EmployeeId.ToString());
            }
        }
    }
}