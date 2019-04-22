namespace NexGoal.Criteria.FlowSetting
{
    using Fanex.Data.Repository;

    public class GetGroupsCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "GroupFlowSetting_GetGroups";
    }
}