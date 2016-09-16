namespace NexGoal.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using NexGoal.Model.Report;
    using NexGoal.Model.Report.EPADetail;
    using NexGoal.Model.Report.EPASummary;
    using System.Collections.Generic;

    public interface IReportRepository
    {
        EPASummaryModel GetEPASummary(ICriteria criteria);

        EPADetailModel GetEPADetail(ICriteria criteria);

        EPADetailModel GetMonthlyEPADetail(ICriteria criteria);

        EPASummaryModel GetMonthlyEPASummary(ICriteria criteria);

        IEnumerable<MasterReportModel> GetMaster(ICriteria criteria);
    }
}