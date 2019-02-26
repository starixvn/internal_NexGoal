namespace NexGoal.Services.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using NexGoal.Models.PARecord;

    public interface IExportPAReportService
    {
        Task<byte[]> ExportExcel(GetPARecordMultiPeriodParam param);

        Task<Tuple<string, byte[]>> ExportPADetailPDF(int actorId, int paRecordId);
    }
}