namespace NexGoal.BackgroundJob.Criterias.PAFocus
{
    using NexGoal.Core.Criteria;

    public class GenerateCriteria : CommandBaseCriteria
    {
        public string PeriodIds { get; set; }

        public override string GetSettingKey() => "Auto_PAFocus_Generate";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(PeriodIds);
    }
}