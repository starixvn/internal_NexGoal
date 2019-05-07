namespace NexGoal.Criteria.PRTracking
{
    using Fanex.Data.Repository;

    public class GetGroupsAndMembersCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public override string GetSettingKey() => "PRTracking_GetGroupsAndMembers";

        public override bool IsValid() => ActorId > 0;
    }
}