namespace NexGoal.Criteria.PMSetting
{
    public class RemoveMemberCriteria : CommandBaseCriteria
    {
        public int ProjectManagerId { get; set; }

        public int MemberId { get; set; }

        public int DeletedBy { get; set; }

        public override string GetSettingKey() => "PMSetting_Delete";

        public override bool IsValid() => MemberId > 0 && ProjectManagerId > 0 && DeletedBy > 0;
    }
}