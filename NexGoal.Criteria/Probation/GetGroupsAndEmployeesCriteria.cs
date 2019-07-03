namespace NexGoal.Criteria.Probation
{
    using Fanex.Data.Repository;

    public class GetGroupsAndEmployeesCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public override string GetSettingKey() => "ProbationReport_GetGroupsAndEmployees";

        public override bool IsValid() => ActorId > 0;
    }
}