namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetGroupByActorIdCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public override string GetSettingKey() => "Employee_GetGroup";

        public override bool IsValid() => ActorId > 0;
    }
}