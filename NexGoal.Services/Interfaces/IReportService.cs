namespace NexGoal.Services.Interfaces
{
    using Model;
    using NexGoal.Criteria.Report;
    using NexGoal.Model.Dashboard;
    using NexGoal.Model.Report;
    using NexGoal.Model.Report.EPADetail;
    using System.Collections.Generic;

    public interface IReportService
    {
        DashboardModel GetDashboard(GetDashboardCriteria criteria);

        IEnumerable<EPADetailModel> GetEPADetailReport(GetPAReportCriteria criteria);

        IEnumerable<MasterReportModel> GetMaster(GetMasterReportCriteria criteria);

        IEnumerable<NominationViewModel> GetNomination(GetNominationReportCriteria criteria);
    }
}