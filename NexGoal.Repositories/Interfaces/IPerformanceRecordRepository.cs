namespace NexGoal.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord;

    public interface IPerformanceRecordRepository
    {
        Task<Tuple<IEnumerable<PerformanceRecord>, IEnumerable<ProjectModel>>> GetPerformanceRecordByEmployeeAsync(ICriteria criteria);

        Task<IEnumerable<PerformanceRecord>> GetPerformanceTimelineAsync(ICriteria criteria);

        Task<PerformanceRecord> GetPerformanceRecordByIdAsync(ICriteria criteria);

        Task<PerformanceRecord> GetDraftPerformanceRecordByIdAsync(ICriteria criteria);

        Task<Tuple<IEnumerable<PerformanceRecord>, IEnumerable<ProjectModel>>> GetPerformanceRecordByIdsAsync(ICriteria criteria);

        Task<Tuple<IEnumerable<PerformanceRecord>, IEnumerable<Employee>, IEnumerable<ProjectModel>>> GetMemberPerformanceAsync(ICriteria criteria);

        Task<Tuple<IEnumerable<PerformanceRecord>, IEnumerable<Employee>, IEnumerable<ProjectModel>>> GetMemberPerformanceDetailAsync(ICriteria criteria);

        Task<Tuple<IEnumerable<MemberPerformanceTimelineModel>, IEnumerable<PerformanceRecord>>> GetMemberPerformanceTimelineAsync(ICriteria criteria);
    }
}
