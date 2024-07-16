namespace NexGoal.BackgroundJob.Criterias.Period
{
    using NexGoal.Core.Criteria;

    public class CreatePeriodCriteria : CommandBaseCriteria
    {
        public override string GetSettingKey() => "Auto_Period_Insert";

        public override bool IsValid() => true;
    }
}