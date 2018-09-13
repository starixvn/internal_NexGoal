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
        Task<IEnumerable<UserModel>> GetCreatorsAsync(int employeeId);

        Task<IEnumerable<UserModel>> GetMembersAsync(int actorId);

        Task<Tuple<IEnumerable<PerformanceRecordModel>, int>> GetPerformanceCheckInByEmployeeAsync(GetPerformanceCheckInByEmployeeParam param);

        Task<PerformanceRecordModel> GetPerformanceCheckInByIdAsync(
            int performanceRecordId,
            int creatorId);

        Task<CommandResult> CreatePerformanceCheckInAsync(CreatePerformanceRecordModel performanceRecord);

        Task<CommandResult> EditPerformanceCheckInAsync(EditPerformanceRecordModel performanceRecord);
    }
}