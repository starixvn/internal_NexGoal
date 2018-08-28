namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceCheckIn;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.UserManagement;

    public class PerformanceCheckInService : IPerformanceCheckInService
    {
        private readonly IBaseService baseService;
        private readonly IAvatarService avatarService;
        private readonly ICryptoService cryptoService;

        public PerformanceCheckInService(
            IBaseService baseService,
            IAvatarService avatarService,
            ICryptoService cryptoService)
        {
            this.baseService = baseService;
            this.avatarService = avatarService;
            this.cryptoService = cryptoService;
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
            int? creatorId = null,
            string projectIds = null,
            int? pageSize = null,
            int? pageIndex = null)
        {
            var criteria = new GetPerformanceCheckInByEmployeeCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate,
                CreatorId = creatorId,
                ProjectIds = projectIds,
                PageSize = pageSize,
                PageIndex = pageIndex
            };
            var performanceRecords = await baseService.GetDataAsync<PerformanceRecordModel>(criteria).ConfigureAwait(false);
            foreach (PerformanceRecordModel performanceRecord in performanceRecords)
            {
                performanceRecord.GradeId = cryptoService.Encrypt(performanceRecord.GradeId);

                performanceRecord.Base64Image = avatarService.GetBase64Image(performanceRecord.Code);
                performanceRecord.Base64ImageOfCreator = avatarService.GetBase64Image(performanceRecord.CodeOfCreator);
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