namespace NexGoal.Services
{
    using System;
    using System.Threading.Tasks;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Report;

    public interface IExportPAReportService
    {
        Task<byte[]> ExportExcelAsync(GetPARecordMultiPeriodParam param);

        Task<Tuple<string, byte[]>> ExportPdfAsync(PAReportDetail reportDetail);
    }
}