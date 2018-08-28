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
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.UserManagement;

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

        public async Task<IEnumerable<UserModel>> GetCreatorsByEmployeeAsync(int employeeId)
        {
            var criteria = new GetCreatorByEmployeeCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<UserModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PerformanceRecordModel>> GetPerformanceCheckInByEmployeeAsync(
            int employeeId,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string creatorIds = null,
            string projectIds = null,
            int? pageSize = null,
            int? pageIndex = null)
        {
            var criteria = new GetPerformanceCheckInByEmployeeCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate,
                CreatorIds = creatorIds,
                ProjectIds = projectIds,
                PageSize = pageSize,
                PageIndex = pageIndex
            };
            var performanceRecords = await baseService.GetDataAsync<PerformanceRecordModel>(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
            IEnumerable<UserModel> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);

            foreach (PerformanceRecordModel performanceRecord in performanceRecords)
            {
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

            return performanceRecords;
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
    }
}