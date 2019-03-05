namespace NexGoal.AutoService.Criterias.PA
{
    using NexGoal.Core.Criteria;

    public class EditGradeCriteria : CommandBaseCriteria
    {
        public string Value { get; set; }

        public override string GetSettingKey() => "Auto_PA_UpdateGrade";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Value);
    }
}