namespace NexGoal.Criteria.Periods
{
    using Fanex.Data.Repository;

    public class GetCurrentPerformancePeriodCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Period_GetCurrentPerformancePeriod";

        public override bool IsValid() => true;
    }
}