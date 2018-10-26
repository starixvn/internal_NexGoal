namespace NexGoal.Criteria.PerformanceRecord
{
    using Fanex.Data.Repository;

    public class GetMemberPerformanceCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int? EmployeeId { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_GetMemberPerformance";

        public override bool IsValid() => ActorId > 0;
    }
}