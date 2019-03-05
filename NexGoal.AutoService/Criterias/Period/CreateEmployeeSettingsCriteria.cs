namespace NexGoal.AutoService.Criterias.Period
{
    using NexGoal.Core.Criteria;

    public class CreateEmployeeSettingsCriteria : CommandBaseCriteria
    {
        public string PeriodIds { get; set; }

        public override string GetSettingKey() => "Auto_Period_InsertEmployeeSettings";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(PeriodIds);
    }
}