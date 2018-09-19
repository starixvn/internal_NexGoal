namespace NexGoal.AutoService.Criterias.Period
{
    using Fanex.Data.Repository;

    public class GetPeriodStartedCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "GetPeriodStarted";

        public override bool IsValid() => true;
    }
}