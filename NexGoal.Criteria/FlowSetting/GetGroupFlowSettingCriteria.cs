namespace NexGoal.Criteria.FlowSetting
{
    using Fanex.Data.Repository;

    public class GetGroupFlowSettingCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "GroupFlowSetting_Get";
    }
}