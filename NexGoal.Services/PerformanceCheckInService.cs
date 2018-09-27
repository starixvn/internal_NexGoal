namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceCheckIn;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord;

    public class PerformanceCheckInService : IPerformanceCheckInService
    {
        private readonly IBaseService baseService;
        private readonly IAvatarService avatarService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;

        public PerformanceCheckInService(
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

        public async Task<IEnumerable<EmployeeBaseModel>> GetCreatorsAsync(int employeeId)
        {
            var criteria = new GetCreatorCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<EmployeeBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<EmployeeBaseModel>> GetMembersAsync(int actorId)
        {
            var criteria = new GetMemberCriteria
            {
                ActorId = actorId
            };

            return await baseService.GetDataAsync<EmployeeBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<Tuple<IEnumerable<PerformanceRecordModel>, int>> GetPerformanceCheckInByEmployeeAsync(GetPerformanceCheckInByEmployeeParam param)
        {
            var criteria = new GetPerformanceCheckInByEmployeeCriteria
            {
                EmployeeId = param.EmployeeId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                CreatorIds = param.CreatorIds,
                ProjectIds = param.ProjectIds,
                PageSize = param.PageSize,
                PageIndex = param.PageIndex
            };
            var performanceRecords = await baseService.GetDataAsync<PerformanceRecordModel>(criteria).ConfigureAwait(false);
            IEnumerable<ProjectModel> projects = await commonService.GetProjectsAsync().ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
            IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);

            foreach (PerformanceRecordModel performanceRecord in performanceRecords)
            {
                MappingData(performanceRecord, projects, performanceGrades, employees);
            }

            return new Tuple<IEnumerable<PerformanceRecordModel>, int>(performanceRecords, criteria.TotalItems);
        }

        public async Task<PerformanceRecordModel> GetPerformanceCheckInByIdAsync(
            int performanceRecordId,
            int creatorId)
        {
            var criteria = new GetPerformanceCheckInByIdCriteria
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

                MappingData(performanceRecord, projects, performanceGrades, employees);
            }

            return performanceRecord;
        }

        public async Task<CommandResult> CreatePerformanceCheckInAsync(CreatePerformanceRecordModel performanceRecord)
        {
            var criteria = new CreatePerformanceCheckInCriteria
            {
                EmployeeId = performanceRecord.EmployeeId,
                CreatorId = performanceRecord.CreatorId,
                GradeId = cryptoService.Encrypt(performanceRecord.GradeId),
                Comment = performanceRecord.Comment,
                ProjectIds = performanceRecord.ProjectIds,
                StatusId = performanceRecord.StatusId,
                RoleId = performanceRecord.RoleId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditPerformanceCheckInAsync(EditPerformanceRecordModel performanceRecord)
        {
            var criteria = new EditPerformanceCheckInCriteria
            {
                PerformanceRecordId = performanceRecord.PerformanceRecordId,
                EmployeeId = performanceRecord.EmployeeId,
                CreatorId = performanceRecord.CreatorId,
                GradeId = cryptoService.Encrypt(performanceRecord.GradeId),
                Comment = performanceRecord.Comment,
                ProjectIds = performanceRecord.ProjectIds,
                StatusId = performanceRecord.StatusId,
                RoleId = performanceRecord.RoleId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        private void MappingData(
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
    }
}