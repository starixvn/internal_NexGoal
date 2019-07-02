namespace NexGoal.Criteria.Probation
{
    using Fanex.Data.Repository;

    public class GetProbationersNotInEvaluationCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Probation_GetProbationersNotInEvaluation";

        public override bool IsValid() => true;
    }
}