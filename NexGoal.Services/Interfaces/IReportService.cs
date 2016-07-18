namespace NexGoal.Services.Interfaces
{
    using System.IO;
    using NexGoal.Criteria.Report;
    using NexGoal.Model.Report.Dashboard;
    using NexGoal.Model.Report.EPADetail;
    using NexGoal.Model.Report.EPASummary;

    public interface IReportService
    {
        DashboardModel GetDashboard(GetDashboardCriteria criteria);

        EPASummaryModel GetEPASummaryReport(GetEPASummaryCriteria criteria);

        MemoryStream ExportEPASummaryReport(EPASummaryModel epaSummary);

        MemoryStream ExportMonthlyEPASummaryReport(EPASummaryModel epaSummary);

        EPADetailModel GetEPADetailReport(GetEPADetailCriteria criteria);

        EPADetailModel GetMonthlyEPADetailReport(GetMonthlyEPADetailCriteria criteria);

        void ExportMonthlyEPADetailReport(EPADetailModel epaDetail, string templatePath);

        EPASummaryModel GetMonthlyEPASummaryReport(GetMonthlyEPASummaryCriteria criteria);
    }
}