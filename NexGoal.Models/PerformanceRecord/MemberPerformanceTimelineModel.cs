namespace NexGoal.Models.PerformanceRecord
{
    using System.Collections.Generic;
    using NexGoal.Models.Common;

    public class MemberPerformanceTimelineModel : EmployeeBaseModel
    {
        public IEnumerable<PerformanceRecordModel> PerformanceRecords { get; set; } = new List<PerformanceRecordModel>();

        public int TotalItems { get; set; }
    }
}