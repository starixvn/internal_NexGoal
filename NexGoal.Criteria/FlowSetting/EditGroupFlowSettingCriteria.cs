namespace NexGoal.Criteria.FlowSetting
{
    public class EditGroupFlowSettingCriteria : CommandBaseCriteria
    {
        public int GroupId { get; set; }
        public string FlowDetails { get; set; }
        public int ActorId { get; set; }

        public override string GetSettingKey() => "GroupFlowSetting_Update";

        public override bool IsValid() => GroupId > 0 && ActorId > 0;
    }
}