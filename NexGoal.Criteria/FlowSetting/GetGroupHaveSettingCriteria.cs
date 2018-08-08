namespace NexGoal.Criteria.FlowSetting
{
    using Fanex.Data.Repository;

    public class GetGroupHaveSettingCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public override bool IsValid() => FlowTemplateId > 0;

        public override string GetSettingKey() => "GroupFlowSetting_GetGroupHaveSetting";
    }
}