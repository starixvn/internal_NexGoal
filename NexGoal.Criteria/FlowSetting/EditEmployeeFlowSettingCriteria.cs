namespace NexGoal.Criteria.FlowSetting
{
    public class EditEmployeeFlowSettingCriteria : CommandBaseCriteria
    {
        public int EmployeeId { get; set; }
        public int FlowTemplateId { get; set; }
        public string FlowDetails { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "EmployeeFlowSetting_Update";

        public override bool IsValid() => EmployeeId > 0 && FlowTemplateId > 0 && ModifiedBy > 0;
    }
}