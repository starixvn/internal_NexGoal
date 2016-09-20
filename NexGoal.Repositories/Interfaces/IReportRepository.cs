namespace NexGoal.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using NexGoal.Model.Report;
    using NexGoal.Model.Report.EPADetail;
    using System.Collections.Generic;

    public interface IReportRepository
    {
        IEnumerable<EPADetailModel> GetEPADetail(ICriteria criteria);

        IEnumerable<MasterReportModel> GetMaster(ICriteria criteria);
    }
}