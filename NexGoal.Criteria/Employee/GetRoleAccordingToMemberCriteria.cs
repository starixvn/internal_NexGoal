namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetRoleAccordingToMemberCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int MemberId { get; set; }

        public override string GetSettingKey() => "Employee_GetRoleAccordingToMember";

        public override bool IsValid() => ActorId > 0 && MemberId > 0;
    }
}