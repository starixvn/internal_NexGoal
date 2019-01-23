namespace NexGoal.Criteria.PerformanceRecord
{
    using Fanex.Data.Repository;

    public class GetMemberPerformanceDetailCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_GetMemberPerformanceByMemberId";

        public override bool IsValid() => ActorId > 0 && EmployeeId > 0;
    }
}