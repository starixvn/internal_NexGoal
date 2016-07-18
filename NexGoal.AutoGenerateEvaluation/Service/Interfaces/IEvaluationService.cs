namespace NexGoal.AutoGenerateEvaluation.Service.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.AutoGenerateEvaluation.Criteria;
    using NexGoal.AutoGenerateEvaluation.Criteria.Monthly;
    using NexGoal.Core.Model;
    using NexGoal.Model.Period;

    public interface IEvaluationService
    {
        IEnumerable<int> CheckStartSemiAnnual(GetSemiAnnualPeriodStartedCriteria criteria);

        CommandResult InsertEmployeeToSemiAnnualPeriod(InsertEmployeeToSemiAnnualPeriodCriteria criteria);

        AutoEmployeePeriodModel GetEmployeeProbationEvaluation(GetEmployeeProbationEvaluationCriteria criteria);

        CommandResult InsertEmployeeToProbationPeriod(InsertEmployeeToProbationPeriodCriteria criteria);

        IEnumerable<AutoEmployeePeriodModel> GetPeriodHasReminderProcess(GetPeriodHasReminderProcessCriteria criteria);

        IEnumerable<AutoEmployeePeriodModel> GetEmployeePromotionEvaluation(GetEmployeePromotionEvaluationCriteria criteria);

        IEnumerable<AutoEmployeePeriodModel> CheckEndProcess(CheckEndProcessCriteria criteria);

        CommandResult UpdateEndProcess(UpdateIsEndProcessCriteria criteria);

        CommandResult UpdateFinalScoreEndEvaluation(UpdateFinalScoreEndEvaluationCriteria criteria);

        int CheckLastestProcessOfPeriod(CheckLastestProcessOfPeriodCriteria criteria);

        CommandResult UpdateTotalScoreOfProcess(UpdateTotalScoreOfProcessCriteria criteria);

        CommandResult UpdateTotalScoreOfSemiHasMonthly(UpdateTotalScoreOfSemiHasMonthlyCriteria criteria);

        #region Monthly

        IEnumerable<int> GetMonthlyPeriodStarted(GetMonthlyPeriodStartedCriteria criteria);

        CommandResult InsertEmployeeToMonthlyPeriod(InsertEmployeeToMonthlyPeriodCriteria criteria);

        #endregion Monthly
    }
}