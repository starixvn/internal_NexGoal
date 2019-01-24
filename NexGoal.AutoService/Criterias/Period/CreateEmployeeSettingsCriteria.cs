namespace NexGoal.AutoService.Criterias.Period
{
    public class CreateEmployeeSettingsCriteria : CommandBaseCriteria
    {
        public string PeriodIds { get; set; }

        public override string GetSettingKey() => "InsertEmployeeSettings";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(PeriodIds);
    }
}