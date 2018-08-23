namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.PerformanceRecord;

    public interface IPerformanceCheckInService
    {
        Task<IEnumerable<PerformanceRecordModel>> GetPerformanceCheckInByEmployeeIdAsync(
            int employeeId,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? creatorId = null,
            string projectIds = null,
            int? pageSize = null,
            int? pageIndex = null);
    }
}