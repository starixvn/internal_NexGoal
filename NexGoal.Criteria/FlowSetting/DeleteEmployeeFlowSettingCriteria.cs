namespace NexGoal.Criteria.FlowSetting
{
    public class DeleteEmployeeFlowSettingCriteria : CommandBaseCriteria
    {
        public int EmployeeId { get; set; }
        public int FlowTemplateId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "EmployeeFlowSetting_Delete";

        public override bool IsValid() => EmployeeId > 0 && FlowTemplateId > 0 && ModifiedBy > 0;
    }
}