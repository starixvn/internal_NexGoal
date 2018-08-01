namespace NexGoal.Criteria.FlowTemplate
{
    public class CreateFlowTemplateCriteria : CommandBaseCriteria
    {
        public string Name { get; set; }
        public int TargetTypeId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "FlowTemplate_Insert";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Name) && TargetTypeId > 0 && CreatedBy > 0;
    }
}