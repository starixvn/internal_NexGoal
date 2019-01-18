namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.PARecord;

    public interface IPAReportRepository
    {
        Task<IEnumerable<PARecordModel>> GetPAReportAsync(ICriteria criteria);
    }
}