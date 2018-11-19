namespace NexGoal.AutoService.Criterias.PA
{
    public class EditGradeCriteria : CommandBaseCriteria
    {
        public string Value { get; set; }

        public override string GetSettingKey() => "EditPAGrade";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Value);
    }
}