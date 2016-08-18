namespace NexGoal.Criteria.Period
{
    using Fanex.Data.Repository;

    public class GetAllPeriodByEvaluatorCriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }

        public override string GetSettingKey()
        {
            return "Period_GetAllByEvaluator";
        }
    }
}