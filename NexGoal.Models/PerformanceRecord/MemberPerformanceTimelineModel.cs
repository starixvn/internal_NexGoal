namespace NexGoal.Models.PerformanceRecord
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using NexGoal.Models.Common;

    public class MemberPerformanceTimelineModel : EmployeeBaseModel
    {
        public IEnumerable<PerformanceRecordModel> PerformanceRecords { get; set; } = new List<PerformanceRecordModel>();

        public int TotalItems { get; set; }

        [JsonIgnore]
        public string GradeId { get; set; }
        public PerformanceGradeModel Grade { get; set; }
    }
}