namespace NexGoal.AutoGenerateEvaluation
{
    using NexGoal.Core.Model;
    using NexGoal.Model.Period;
    using System.Collections.Generic;

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
    }
}