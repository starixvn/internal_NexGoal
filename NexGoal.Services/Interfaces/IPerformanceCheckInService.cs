namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.UserManagement;

    public interface IPerformanceCheckInService
    {
        Task<IEnumerable<UserModel>> GetCreatorsByEmployeeAsync(int employeeId);

        Task<IEnumerable<PerformanceRecordModel>> GetPerformanceCheckInByEmployeeAsync(
            int employeeId,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string creatorIds = null,
            string projectIds = null,
            int? pageSize = null,
            int? pageIndex = null);

        Task<CommandResult> CreatePerformanceCheckInAsync(CreatePerformanceRecordModel performanceRecord);
    }
}