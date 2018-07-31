namespace NexGoal.Criteria.FlowTemplate
{
    using Fanex.Data.Repository;

    public class GetFlowTemplateByIdCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public override string GetSettingKey() => "FlowTemplate_GetById";

        public override bool IsValid() => FlowTemplateId > 0;
    }
}