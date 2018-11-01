namespace NexGoal.Criteria.PerformanceRecord
{
    using Fanex.Data.Repository;

    public class GetMemberPerformanceTimelineCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int? PeriodId { get; set; }
        public int? EmployeeId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_GetMemberTimeline";

        public override bool IsValid() => ActorId > 0;
    }
}