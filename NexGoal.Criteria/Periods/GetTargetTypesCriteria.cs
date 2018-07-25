namespace NexGoal.Criteria.Periods
{
    using Fanex.Data.Repository;

    public class GetTargetTypesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "TargetType_Get";

        public override bool IsValid() => true;
    }
}