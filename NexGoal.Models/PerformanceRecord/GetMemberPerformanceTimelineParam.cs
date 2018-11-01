namespace NexGoal.Models.PerformanceRecord
{
    public class GetMemberPerformanceTimelineParam
    {
        public int ActorId { get; set; }
        public int? PeriodId { get; set; }
        public int? EmployeeId { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}