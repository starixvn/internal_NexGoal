namespace NexGoal.AutoService.Criterias.PAFocus
{
    public class GenerateCriteria : CommandBaseCriteria
    {
        public string PeriodIds { get; set; }

        public override string GetSettingKey() => "PAFocus_Generate";

        public override bool IsValid() => true;
    }
}