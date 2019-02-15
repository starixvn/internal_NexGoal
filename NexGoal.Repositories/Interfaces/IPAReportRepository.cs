namespace NexGoal.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;

    public interface IPAReportRepository
    {
        Task<Tuple<IEnumerable<PARecordModel>, IEnumerable<PeriodBaseModel>>> GetPAReportAsync(ICriteria criteria);
    }
}