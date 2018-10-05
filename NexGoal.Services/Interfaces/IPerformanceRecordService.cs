namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord;

    public interface IPerformanceRecordService
    {
        Task<IEnumerable<EmployeeBaseModel>> GetCreatorsAsync(int employeeId);

        Task<IEnumerable<EmployeeBaseModel>> GetMembersAsync(int actorId);

        Task<Tuple<IEnumerable<PerformanceRecordModel>, int>> GetPerformanceRecordByEmployeeAsync(GetPerformanceRecordByEmployeeParam param);

        Task<PerformanceRecordModel> GetPerformanceRecordByIdAsync(
            int performanceRecordId,
            int creatorId);

        Task<CommandResult> CreatePerformanceRecordAsync(CreatePerformanceRecordModel performanceRecord);

        Task<CommandResult> EditPerformanceRecordAsync(EditPerformanceRecordModel performanceRecord);
    }
}