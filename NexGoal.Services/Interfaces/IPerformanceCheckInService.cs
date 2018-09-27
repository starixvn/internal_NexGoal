namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord;

    public interface IPerformanceCheckInService
    {
        Task<IEnumerable<EmployeeBaseModel>> GetCreatorsAsync(int employeeId);

        Task<IEnumerable<EmployeeBaseModel>> GetMembersAsync(int actorId);

        Task<Tuple<IEnumerable<PerformanceRecordModel>, int>> GetPerformanceCheckInByEmployeeAsync(GetPerformanceCheckInByEmployeeParam param);

        Task<PerformanceRecordModel> GetPerformanceCheckInByIdAsync(
            int performanceRecordId,
            int creatorId);

        Task<CommandResult> CreatePerformanceCheckInAsync(CreatePerformanceRecordModel performanceRecord);

        Task<CommandResult> EditPerformanceCheckInAsync(EditPerformanceRecordModel performanceRecord);
    }
}