namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceCheckIn;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.UserManagement;

    public class PerformanceCheckInService : IPerformanceCheckInService
    {
        private readonly IBaseService baseService;
        private readonly IAvatarService avatarService;

        public PerformanceCheckInService(
            IBaseService baseService,
            IAvatarService avatarService)
        {
            this.baseService = baseService;
            this.avatarService = avatarService;
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
                performanceRecord.Base64Image = avatarService.GetBase64Image(performanceRecord.Code);
                performanceRecord.Base64ImageOfCreator = avatarService.GetBase64Image(performanceRecord.CodeOfCreator);
            }

            return performanceRecords;
        }
    }
}