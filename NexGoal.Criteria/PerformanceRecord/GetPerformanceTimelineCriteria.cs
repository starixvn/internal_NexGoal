namespace NexGoal.Criteria.PerformanceRecord
{
    using Fanex.Data.Repository;

    public class GetPerformanceTimelineCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_GetTimeline";

        public override bool IsValid() => EmployeeId > 0;
    }
}