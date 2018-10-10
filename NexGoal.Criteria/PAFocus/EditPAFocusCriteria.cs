namespace NexGoal.Criteria.PAFocus
{
    public class EditPAFocusCriteria : CommandBaseCriteria
    {
        public int ActorId { get; set; }
        public string Value { get; set; }

        public override string GetSettingKey() => "PAFocus_Update";

        public override bool IsValid() => ActorId > 0 && !string.IsNullOrWhiteSpace(Value);
    }
}