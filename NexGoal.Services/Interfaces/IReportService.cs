namespace NexGoal.Services.Interfaces
{
    using NexGoal.Criteria.Report;
    using NexGoal.Model.Report;
    using NexGoal.Model.Report.Dashboard;
    using NexGoal.Model.Report.EPADetail;
    using NexGoal.Model.Report.EPASummary;
    using System.Collections.Generic;
    using System.IO;

    public interface IReportService
    {
        DashboardModel GetDashboard(GetDashboardCriteria criteria);

        EPASummaryModel GetEPASummaryReport(GetEPASummaryCriteria criteria);

        MemoryStream ExportEPASummaryReport(EPASummaryModel epaSummary);

        MemoryStream ExportMonthlyEPASummaryReport(EPASummaryModel epaSummary);

        EPADetailModel GetEPADetailReport(GetPAReportCriteria criteria);

        void ExportMonthlyEPADetailReport(EPADetailModel epaDetail, string templatePath);

        IEnumerable<MasterReportModel> GetMaster(GetMasterReportCriteria criteria);
    }
}