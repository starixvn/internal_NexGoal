namespace NexGoal.BackgroundJob.Criteria.Period
{
    using Fanex.Data.Repository;

    public class GetPeriodStartedCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Auto_Period_GetPeriodStarted";

        public override bool IsValid() => true;
    }
}