namespace NexGoal.AutoService.Criterias.Period
{
    public class CreatePeriodCriteria : CommandBaseCriteria
    {
        public override string GetSettingKey() => "InsertPeriod";

        public override bool IsValid() => true;
    }
}