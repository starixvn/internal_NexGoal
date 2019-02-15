namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Report;

    public interface IPAReportService
    {
        Task<IEnumerable<PARecordGroupByPeriod>> GetPAReportAsync(GetPARecordMultiPeriodParam param);

        Task<PAReportDetail> GetPAReportDetailAsync(int actorId, int paRecordId);
    }
}