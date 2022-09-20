namespace NexGoal.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.PerformanceRecord;

    public interface IPerformanceRecordRepository
    {
        Task<IEnumerable<PerformanceRecord>> GetPerformanceRecordByEmployeeAsync(ICriteria criteria);

        Task<IEnumerable<PerformanceRecord>> GetPerformanceTimelineAsync(ICriteria criteria);

        Task<PerformanceRecord> GetPerformanceRecordByIdAsync(ICriteria criteria);

        Task<PerformanceRecord> GetDraftPerformanceRecordByIdAsync(ICriteria criteria);

        Task<IEnumerable<PerformanceRecord>> GetPerformanceRecordByIdsAsync(ICriteria criteria);

        Task<IEnumerable<PerformanceRecord>> GetMemberPerformanceAsync(ICriteria criteria);

        Task<IEnumerable<PerformanceRecord>> GetMemberPerformanceDetailAsync(ICriteria criteria);

        Task<ValueTuple<IEnumerable<MemberPerformanceTimelineModel>, IEnumerable<PerformanceRecord>>> GetMemberPerformanceTimelineAsync(ICriteria criteria);
    }
}