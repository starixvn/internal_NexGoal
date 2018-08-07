namespace NexGoal.Criteria.FlowSetting
{
    using Fanex.Data.Repository;

    public class GetGroupHasSettingCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public override bool IsValid() => FlowTemplateId > 0;

        public override string GetSettingKey() => "FlowTemplate_GetGroupHasSetting";
    }
}