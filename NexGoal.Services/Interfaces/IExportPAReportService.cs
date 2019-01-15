namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Report;

    public interface IExportPAReportService
    {
        byte[] ExportExcel(IEnumerable<PARecordGroupByPeriod> paRecords);

        byte[] ExportPADetail(PARecordModel record);
    }
}