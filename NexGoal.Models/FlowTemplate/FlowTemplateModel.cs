namespace NexGoal.Models.FlowTemplate
{
    public class FlowTemplateModel
    {
        public int FlowTemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public int TargetTypeId { get; set; }
        public string TargetTypeName { get; set; }
    }
}