namespace NexGoal.Criteria.FlowSetting
{
    public class EditGroupFlowSettingCriteria : CommandBaseCriteria
    {
        public int GroupId { get; set; }
        public int FlowTemplateId { get; set; }
        public string FlowDetails { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "GroupFlowSetting_Update";

        public override bool IsValid() => GroupId > 0 && FlowTemplateId > 0 && ModifiedBy > 0;
    }
}