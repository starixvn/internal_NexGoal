namespace NexGoal.Criteria.Periods
{
    using NexGoal.Core.Criteria;

    public class EditFinalPAGradeCriteria : CommandBaseCriteria
    {
        public int ActorId { get; set; }
        public string Value { get; set; }

        public override string GetSettingKey() => "Period_UpdateFinalPAGrade";

        public override bool IsValid() => ActorId > 0 && !string.IsNullOrWhiteSpace(Value);
    }
}