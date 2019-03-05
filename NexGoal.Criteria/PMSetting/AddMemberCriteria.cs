namespace NexGoal.Criteria.PMSetting
{
    using NexGoal.Core.Criteria;

    public class AddMemberCriteria : CommandBaseCriteria
    {
        public int ProjectManagerId { get; set; }
        public string MemberIds { get; set; }
        public int ActorId { get; set; }

        public override string GetSettingKey() => "PMSetting_Insert";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MemberIds) && ProjectManagerId > 0 && ActorId > 0;
    }
}