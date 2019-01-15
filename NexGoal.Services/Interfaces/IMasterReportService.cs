namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.Common;
    using NexGoal.Models.MasterReport;

    public interface IMasterReportService
    {
        Task<IEnumerable<PeriodBaseModel>> GetPeriods();

        Task<IEnumerable<MasterReportModel>> GetRecords(int periodId, string groupIds, string employeeIds);
    }
}