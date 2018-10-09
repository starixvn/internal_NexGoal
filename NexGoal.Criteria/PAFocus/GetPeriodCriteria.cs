namespace NexGoal.Criteria.PAFocus
{
    using Fanex.Data.Repository;

    public class GetPeriodCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "PAFocus_GetPeriod";
    }
}