namespace NexGoal.AutoService.Criterias.Period
{
    public class GeneratePACriteria : CommandBaseCriteria
    {
        public string PeriodIds { get; set; }

        public override string GetSettingKey() => "GeneratePA";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(PeriodIds);
    }
}