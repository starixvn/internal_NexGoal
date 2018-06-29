namespace NexGoal.Criteria.Function
{
    public class AssignFunctionToRoleCriteria : CommandBaseCriteria
    {
        public int RoleId { get; set; }
        public string Value { get; set; }
        public int ModifiedBy { get; set; }

        public override bool IsValid() => RoleId > 0 && ModifiedBy > 0 && !string.IsNullOrWhiteSpace(Value);

        public override string GetSettingKey() => "Role_AssignFunction";
    }
}