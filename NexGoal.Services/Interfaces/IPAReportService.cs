namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Report;

    public interface IPAReportService
    {
        Task<Tuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupAndMember(int actorId);

        Task<IEnumerable<PARecordGroupByPeriod>> GetPAReportAsync(GetPARecordMultiPeriodParam param);

        Task<PAReportDetail> GetPAReportDetailAsync(int actorId, int paRecordId);
    }
}