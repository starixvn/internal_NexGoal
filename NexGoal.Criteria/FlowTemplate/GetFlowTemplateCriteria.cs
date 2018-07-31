namespace NexGoal.Criteria.FlowTemplate
{
    using Fanex.Data.Repository;

    public class GetFlowTemplateCriteria : CriteriaBase
    {
        public int? TargetTypeId { get; set; }
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "FlowTemplate_Get";

        public override bool IsValid() => true;
    }
}