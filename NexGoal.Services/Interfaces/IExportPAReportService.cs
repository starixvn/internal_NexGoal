namespace NexGoal.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.Report;

    public interface IExportPAReportService
    {
        byte[] ExportExcel(IEnumerable<PARecordGroupByPeriod> paRecords);

        Task<Tuple<string, byte[]>> ExportPADetailPDF(int actorId, int paRecordId);
    }
}