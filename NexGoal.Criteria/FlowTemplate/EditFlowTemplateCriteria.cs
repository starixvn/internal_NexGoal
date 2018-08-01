namespace NexGoal.Criteria.FlowTemplate
{
    public class EditFlowTemplateCriteria : CommandBaseCriteria
    {
        public int FlowTemplateId { get; set; }
        public string Name { get; set; }
        public int TargetTypeId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "FlowTemplate_Update";

        public override bool IsValid() => FlowTemplateId > 0 && !string.IsNullOrWhiteSpace(Name) && TargetTypeId > 0 && ModifiedBy > 0;
    }
}