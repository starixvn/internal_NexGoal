namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceCheckIn;
    using NexGoal.Models.PerformanceRecord;

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

        public async Task<IEnumerable<PerformanceRecordModel>> GetPerformanceCheckInByEmployeeIdAsync(
            int employeeId,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? creatorId = null,
            string projectIds = null,
            int? pageSize = null,
            int? pageIndex = null)
        {
            var criteria = new GetPerformanceCheckInByEmployeeIdCriteria
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