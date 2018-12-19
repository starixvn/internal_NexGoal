namespace NexGoal.Criteria.FlowSetting
{
    public class EditEmployeeFlowSettingCriteria : CommandBaseCriteria
    {
        public int EmployeeId { get; set; }
        public string FlowDetails { get; set; }
        public int ActorId { get; set; }

        public override string GetSettingKey() => "EmployeeFlowSetting_Update";

        public override bool IsValid() => EmployeeId > 0 && ActorId > 0;
    }
}