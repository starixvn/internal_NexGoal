namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.PerformanceRecord;

    public interface IPerformanceRecordService
    {
        Task<IEnumerable<PerformanceRecord>> GetPerformanceRecordByEmployeeAsync(int employeeId);

        Task<PerformanceRecord> GetPerformanceRecordByIdAsync(int performanceRecordId, int creatorId);

        Task<PerformanceRecord> GetDraftPerformanceRecordByIdAsync(int performanceRecordId, int creatorId);

        Task<IEnumerable<PerformanceRecord>> GetPerformanceRecordByIdsAsync(string performanceRecordIds, int creatorId);

        Task<ValueTuple<IEnumerable<PerformanceRecord>, int>> GetPerformanceTimelineAsync(GetPerformanceTimelineParam param);

        Task<IEnumerable<PerformanceRecord>> GetMemberPerformanceAsync(int actorId);

        Task<IEnumerable<PerformanceRecord>> GetMemberPerformanceDetailAsync(int actorId, int employeeId);

        Task<IEnumerable<MemberPerformanceTimelineModel>> GetMemberPerformanceTimelineAsync(GetMemberPerformanceTimelineParam param);

        Task<CommandResult> CreatePerformanceRecordAsync(CreatePerformanceRecordDto performanceRecord);

        Task<CommandResult> CreateDraftPerformanceRecordAsync(CreatePerformanceRecordDto performanceRecord);

        Task<CommandResult> EditPerformanceRecordAsync(EditPerformanceRecordDto performanceRecord);

        Task<CommandResult> EditDraftPerformanceRecordAsync(EditPerformanceRecordDto performanceRecord);

        Task<CommandResult> DeletePerformanceRecordAsync(int performanceRecordId, int actorId);
    }
}