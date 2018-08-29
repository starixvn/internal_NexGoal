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

        Task<Tuple<IEnumerable<PerformanceRecordModel>, int>> GetPerformanceCheckInByEmployeeAsync(GetPerformanceCheckInByEmployeeParam param);

        Task<CommandResult> CreatePerformanceCheckInAsync(CreatePerformanceRecordModel performanceRecord);
    }
}