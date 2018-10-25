namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceRecord;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord;

    public class PerformanceRecordService : IPerformanceRecordService
    {
        private readonly IBaseService baseService;
        private readonly IAvatarService avatarService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;

        public PerformanceRecordService(
            IBaseService baseService,
            IAvatarService avatarService,
            ICryptoService cryptoService,
            ICommonService commonService)
        {
            this.baseService = baseService;
            this.avatarService = avatarService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<EmployeeBaseModel>> GetMembersAsync(int actorId)
        {
            var criteria = new GetMemberCriteria
            {
                ActorId = actorId
            };

            return await baseService.GetDataAsync<EmployeeBaseModel>(criteria).ConfigureAwait(false);
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

        public async Task<IEnumerable<MemberPerformanceTimelineModel>> GetMemberPerformanceTimelineAsyn(GetMemberPerformanceTimelineParam param)
        {
            var criteria = new GetMemberPerformanceTimelineCriteria
            {
                ActorId = param.ActorId,
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
                EmployeeId = performanceRecord.EmployeeId,
                CreatorId = performanceRecord.CreatorId,
                GradeId = cryptoService.Encrypt(performanceRecord.GradeId),
                Comment = performanceRecord.Comment,
                ProjectIds = performanceRecord.ProjectIds,
                StatusId = performanceRecord.StatusId,
                TypeId = performanceRecord.TypeId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            commandResult.ObjectId = criteria.PerformanceRecordId;

            return commandResult;
        }

        public async Task<CommandResult> EditPerformanceRecordAsync(EditPerformanceRecordModel performanceRecord)
        {
            var criteria = new EditPerformanceRecordCriteria
            {
                PerformanceRecordId = performanceRecord.PerformanceRecordId,
                EmployeeId = performanceRecord.EmployeeId,
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

            performanceRecord.GradeId = cryptoService.Decrypt(performanceRecord.GradeId);
            int.TryParse(performanceRecord.GradeId, out int gradeId);
            performanceRecord.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

            performanceRecord.Employee = employees.FirstOrDefault(x => x.EmployeeId == performanceRecord.EmployeeId);
            performanceRecord.Creator = employees.FirstOrDefault(x => x.EmployeeId == performanceRecord.CreatorId);

            if (performanceRecord.Employee != null)
            {
                performanceRecord.Employee.Base64Image = avatarService.GetBase64Image(performanceRecord.Employee.Code);
            }

            if (performanceRecord.Creator != null)
            {
                performanceRecord.Creator.Base64Image = avatarService.GetBase64Image(performanceRecord.Creator.Code);
            }
        }

        private void MappingDataPerformanceTimeline(
            PerformanceRecordModel performanceRecord,
            IEnumerable<PerformanceGradeModel> performanceGrades,
            IEnumerable<EmployeeBaseModel> employees)
        {
            performanceRecord.GradeId = cryptoService.Decrypt(performanceRecord.GradeId);
            int.TryParse(performanceRecord.GradeId, out int gradeId);
            performanceRecord.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

            performanceRecord.Creator = employees.FirstOrDefault(x => x.EmployeeId == performanceRecord.CreatorId);
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
                memberPerformance.PerformanceRecords = performanceRecords.Where(x => x.EmployeeId == memberPerformance.EmployeeId);
            }
        }
    }
}