namespace NexGoal.Criteria.FlowManagement
{
    using Fanex.Data.Repository;

    public class GetFlowTemplateCriteria : CriteriaBase
    {
        public int? TargetTypeId { get; set; }

        public bool? IsActive { get; set; }

        public override string GetSettingKey()
        {
            return "FlowTemplate_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}