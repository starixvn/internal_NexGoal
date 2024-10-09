namespace NexGoal.BackgroundJob.Criteria.Period
{
    using NexGoal.Core.Criteria;

    public class GeneratePACriteria : CommandBaseCriteria
    {
        public string PeriodIds { get; set; }

        public override string GetSettingKey() => "Auto_Period_GeneratePA";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(PeriodIds);
    }
}