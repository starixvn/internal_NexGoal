namespace NexGoal.Criteria.FlowSetting
{
    using NexGoal.Core.Criteria;

    public class DeleteGroupFlowSettingCriteria : CommandBaseCriteria
    {
        public int GroupId { get; set; }
        public int ActorId { get; set; }

        public override string GetSettingKey() => "GroupFlowSetting_Delete";

        public override bool IsValid() => GroupId > 0 && ActorId > 0;
    }
}