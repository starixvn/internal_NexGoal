namespace NexGoal.Services.Interfaces
{
    using NexGoal.Model.Report;
    using System.Collections.Generic;

    public interface IMasterExportService
    {
        void Export(IEnumerable<MasterReportModel> evaluations, string templatePath, string periodName);
    }
}
