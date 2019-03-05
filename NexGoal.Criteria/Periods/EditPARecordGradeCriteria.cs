namespace NexGoal.Criteria.Periods
{
    using NexGoal.Core.Criteria;

    public class EditPARecordGradeCriteria : CommandBaseCriteria
    {
        public string Value { get; set; }

        public override string GetSettingKey() => "Period_UpdatePAGrade";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Value);
    }
}