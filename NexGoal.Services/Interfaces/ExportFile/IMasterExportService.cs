namespace NexGoal.Services.Interfaces
{
    using NexGoal.Model.Report;
    using System.Collections.Generic;
    using System.IO;

    public interface IMasterExportService
    {
        MemoryStream Export(IEnumerable<MasterReportModel> evaluations, string periodName);
    }
}
