namespace NexGoal.Criteria.FlowTemplate
{
    public class DeleteFlowTemplateCriteria : CommandBaseCriteria
    {
        public int FlowTemplateId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "FlowTemplate_Delete";

        public override bool IsValid() => FlowTemplateId > 0 && ModifiedBy > 0;
    }
}