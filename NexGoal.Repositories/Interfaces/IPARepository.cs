namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.PARecord;

    public interface IPARepository
    {
        Task<IEnumerable<PerformanceAppraisalResult>> GetPerformanceAppraisalResultAsync(ICriteria criteria);

        Task<IEnumerable<PerformanceAppraisalResult>> GetMemberPerformanceAppraisalResultAsync(ICriteria criteria);
    }
}