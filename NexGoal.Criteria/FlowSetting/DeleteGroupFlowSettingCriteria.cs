namespace NexGoal.Criteria.FlowSetting
{
    public class DeleteGroupFlowSettingCriteria : CommandBaseCriteria
    {
        public int GroupId { get; set; }
        public int FlowTemplateId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "GroupFlowSetting_Delete";

        public override bool IsValid() => GroupId > 0 && FlowTemplateId > 0 && ModifiedBy > 0;
    }
}