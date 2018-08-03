namespace NexGoal.Criteria.FlowSetting
{
    using Fanex.Data.Repository;

    public class GetGroupFlowSettingByIdCriteria : CriteriaBase
    {
        public int GroupId { get; set; }
        public int FlowTemplateId { get; set; }

        public override string GetSettingKey() => "GroupFlowSetting_GetById";

        public override bool IsValid() => GroupId > 0 && FlowTemplateId > 0;
    }
}