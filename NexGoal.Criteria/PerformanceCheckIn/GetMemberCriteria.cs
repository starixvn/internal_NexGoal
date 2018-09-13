namespace NexGoal.Criteria.PerformanceCheckIn
{
    using Fanex.Data.Repository;

    public class GetMemberCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public override string GetSettingKey() => "PerformanceCheckIn_GetMember";

        public override bool IsValid() => ActorId > 0;
    }
}