namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetMemberCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public string Keyword { get; set; }

        public override string GetSettingKey() => "Employee_GetMember";

        public override bool IsValid() => ActorId > 0;
    }
}