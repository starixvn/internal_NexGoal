namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Criteria.EmployeeEPA;
    using NexGoal.Criteria.EmployeeEPA.RatingByKPI;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.EmployeeEPA.RatingByKPI;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.TargetTemplate;

    public interface IEmployeeEPAService
    {
        #region Rating By KPI

        IEnumerable<TargetTemplateModel> GetTargetTemplateToView(GetTargetTemplateToViewCriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIToView(GetKPIToViewCriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(GetEmployeeEPAToViewCriteria criteria);

        IEnumerable<Employee> GetEvaluatorsByTargetTemplate(GetEvaluatorsByTargetTemplateCriteria criteria);

        #endregion Rating By KPI

        IEnumerable<EPAPeriodModel> GetPeriodToView(GetPeriodToViewCriteria criteria);

        IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(GetEPAEndEvaluationPeriodCriteria criteria);

        EPAModel GetEPAInfoById(int epaId);

        IEnumerable<Employee> GetMemberByEvaluatorOrReviewer(GetMemberByEvaluatorOrReviewerCriteria criteria);

        IEnumerable<Group> GetGroupByEvaluatorOrReviewerOrManagement(GetGroupByEvaluatorOrReviewerCriteria criteria);

        IEnumerable<StandardGradeModel> GetStandardGrade();

        IEnumerable<EPASummaryImportModel> GetMemberEPASummaryImport(GetEPASummaryImportCriteria criteria);

        string ConvertScoreToGrade(double score, IEnumerable<StandardGradeModel> standardGrades);

        int CheckEmployeePermission(int epaId, int actorUserId, int? processId = null);

        IEnumerable<EmployeeOtherViolationDetailModel> GetOtherViolationDetailByEPAId(GetOtherViolationDetailByEPAIdCriteria criteria);

        IList<EPAEmployeeModel> GetEmployeeToUpdateFinalScore(GetEmployeeToUpdateFinalScoreCriteria criteria);

        IList<EPAPeriodModel> GetPeriodToUpdateFinalScore(GetPeriodToUpdateFinalScoreCriteria criteria);

        IList<EPAModel> GetEPAToUpdateFinalScore(GetEPAToUpdateFinalScoreCriteria criteria);

        IEnumerable<EPAKPIGroupModel> GetLastRating(int employeeId, int targetTypeId, int epaId);
    }
}