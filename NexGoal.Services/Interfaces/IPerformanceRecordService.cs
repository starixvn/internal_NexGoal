namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord;

    public interface IPerformanceRecordService
    {
        Task<IEnumerable<EmployeeBaseModel>> GetCreatorsAsync(int employeeId);

        Task<IEnumerable<EmployeeBaseModel>> GetMembersAsync(int actorId);

        Task<IEnumerable<PerformanceRecordModel>> GetPerformanceRecordByEmployeeAsync(int employeeId);

        Task<PerformanceRecordModel> GetPerformanceRecordByIdAsync(
            int performanceRecordId,
            int creatorId);

        Task<CommandResult> CreatePerformanceRecordAsync(CreatePerformanceRecordModel performanceRecord);

        Task<CommandResult> EditPerformanceRecordAsync(EditPerformanceRecordModel performanceRecord);
    }
}