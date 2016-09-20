namespace NexGoal.Services.Interfaces
{
    using NexGoal.Criteria.Report;
    using NexGoal.Model.Report;
    using NexGoal.Model.Report.Dashboard;
    using NexGoal.Model.Report.EPADetail;
    using System.Collections.Generic;

    public interface IReportService
    {
        DashboardModel GetDashboard(GetDashboardCriteria criteria);

        IEnumerable<EPADetailModel> GetEPADetailReport(GetPAReportCriteria criteria);

        IEnumerable<MasterReportModel> GetMaster(GetMasterReportCriteria criteria);
    }
}