namespace NexGoal.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using NexGoal.Model.Dashboard;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.EmployeeEPA.RatingByKPI;
    using System.Collections.Generic;

    public interface IEmployeeEPARepository
    {
        IEnumerable<EPAModel> GetEPA(ICriteria criteria);

        EPAModel GetEPAInfo(ICriteria criteria);

        IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(ICriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIToView(ICriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(ICriteria criteria);

        DashboardModel GetDashboard(ICriteria criteria);

        IList<EPAModel> GetEPAToUpdateFinalScore(ICriteria criteria);

        IEnumerable<EPAKPIGroupModel> GetLastRating(ICriteria criteria);
    }
}