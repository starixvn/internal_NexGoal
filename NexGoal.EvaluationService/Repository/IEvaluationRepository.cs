namespace NexGoal.AutoGenerateEvaluation
{
    using Fanex.Data.Repository;
    using NexGoal.Model.Period;

    public interface IEvaluationRepository
    {
        AutoEmployeePeriodModel GetEmployeeProbationEvaluation(ICriteria criteria);
    }
}