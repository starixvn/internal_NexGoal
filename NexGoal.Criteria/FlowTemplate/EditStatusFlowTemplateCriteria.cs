namespace NexGoal.Criteria.FlowTemplate
{
    public class EditStatusFlowTemplateCriteria : CommandBaseCriteria
    {
        public int FlowTemplateId { get; set; }
        public bool IsActive { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "FlowTemplate_UpdateStatus";

        public override bool IsValid() => FlowTemplateId > 0 && ModifiedBy > 0;
    }
}