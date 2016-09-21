namespace NexGoal.Services.Interfaces
{
    using NexGoal.Model.Report;
    using System.Collections.Generic;
    using System.IO;

    public interface IMasterExportService
    {
        void Export(IEnumerable<MasterReportModel> evaluations, string templatePath, string periodName);
    }
}
