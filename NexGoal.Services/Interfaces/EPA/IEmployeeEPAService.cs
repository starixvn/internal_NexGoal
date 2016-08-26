namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.EmployeeEPA;
    using NexGoal.Criteria.EmployeeEPA.RatingByKPI;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.EmployeeEPA.RatingByKPI;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.ScoreRatingType;
    using NexGoal.Model.TargetTemplate;

    public interface IEmployeeEPAService
    {
        #region Rating By KPI

        IEnumerable<TargetTemplateModel> GetTargetTemplateToView(GetTargetTemplateToViewCriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(GetKPIByTargetTemplateCriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIToView(GetKPIToViewCriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(GetEmployeeEPAByKPICriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(GetEmployeeEPAToViewCriteria criteria);

        IEnumerable<Employee> GetEvaluatorsByTargetTemplate(GetEvaluatorsByTargetTemplateCriteria criteria);

        CommandResult UpdateKPIScore(UpdateKPIScoreCriteria criteria);

        #endregion Rating By KPI

        IEnumerable<EPAPeriodModel> GetPeriodToView(GetPeriodToViewCriteria criteria);

        IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(GetEPAEndEvaluationPeriodCriteria criteria);

        EPAModel GetEPAInfoById(int epaId);

        CommandResult UpdateEPAScore(UpdateEPAScoreCriteria criteria);

        CommandResult AdjustScoreByReviewerOrManagement(AdjustScoreByReviewerOrManagementCriteria criteria);

        IEnumerable<Employee> GetMemberByEvaluatorOrReviewer(GetMemberByEvaluatorOrReviewerCriteria criteria);

        IEnumerable<Group> GetGroupByEvaluatorOrReviewerOrManagement(GetGroupByEvaluatorOrReviewerCriteria criteria);

        IEnumerable<StandardGradeModel> GetStandardGrade();

        IEnumerable<EPASummaryImportModel> GetMemberEPASummaryImport(GetEPASummaryImportCriteria criteria);

        CommandResult UpdateEPASummaryImport(UpdateEPASummaryImportCriteria criteria);

        string ConvertScoreToGrade(float score, IEnumerable<StandardGradeModel> standardGrades);

        int CheckEmployeePermission(GetEmployeePermissionCriteria criteria);

        IEnumerable<EmployeeOtherViolationDetailModel> GetOtherViolationDetailByEPAId(GetOtherViolationDetailByEPAIdCriteria criteria);

        IList<EPAModel> GetMonthlyEPAOfSemi(string epaIds);

        CommandResult ResetScore(ResetScoreCriteria criteria);

        int GetNumberOfRow(GetNumberOfRowCriteria criteria);

        void GroupMonthlyEPABySemi(EPAModel semiEPA, IList<EPAModel> monthlyEPAs);

        CommandResult UpdateFinalScoreEPAEndEvaluation(UpdateFinalScoreEPAEndEvaluationCriteria criteria);

        IList<EPAEmployeeModel> GetEmployeeToUpdateFinalScore(GetEmployeeToUpdateFinalScoreCriteria criteria);

        IList<EPAPeriodModel> GetPeriodToUpdateFinalScore(GetPeriodToUpdateFinalScoreCriteria criteria);

        IList<EPAModel> GetEPAToUpdateFinalScore(GetEPAToUpdateFinalScoreCriteria criteria);

        IEnumerable<EPAKPIGroupModel> GetLastRating(int employeeId, int targetTypeId);
    }
}