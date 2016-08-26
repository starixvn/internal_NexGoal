namespace NexGoal.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.EmployeeEPA.RatingByKPI;
    using NexGoal.Model.Report.Dashboard;

    public interface IEmployeeEPARepository
    {
        IEnumerable<EPAModel> GetEPA(ICriteria criteria);

        EPAModel GetEPAInfo(ICriteria criteria);

        IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(ICriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(ICriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIToView(ICriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(ICriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(ICriteria criteria);

        DashboardModel GetDashboard(ICriteria criteria);

        IList<EPAModel> GetEPAToUpdateFinalScore(ICriteria criteria);

        IEnumerable<EPAKPIGroupModel> GetLastRating(ICriteria criteria);
    }
}