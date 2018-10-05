namespace NexGoal.Criteria.PerformanceRecord
{
    using Fanex.Data.Repository;

    public class GetMemberCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_GetMember";

        public override bool IsValid() => ActorId > 0;
    }
}