namespace NexGoal.AutoGenerateEvaluation
{
    using NexGoal.Core.Model;
    using NexGoal.Model.Period;
    using System.Collections.Generic;

    public interface IEvaluationService
    {
        AutoEmployeePeriodModel GetEmployeeProbationEvaluation();

        IEnumerable<AutoEmployeePeriodModel> GetPeriodHasReminderProcess();

        IEnumerable<AutoEmployeePeriodModel> GetEmployeePromotionEvaluation();

        CommandResult UpdateFinalScoreEndEvaluation(UpdateFinalScoreEndEvaluationCriteria criteria);
    }
}