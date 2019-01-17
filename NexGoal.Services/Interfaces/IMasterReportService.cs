namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.MasterReport;

    public interface IMasterReportService
    {
        Task<IEnumerable<MasterReportModel>> GetRecords(int periodId, string groupIds, string employeeIds);

        Task<byte[]> ExportAsync(int periodId, string groupIds, string employeeIds);
    }
}