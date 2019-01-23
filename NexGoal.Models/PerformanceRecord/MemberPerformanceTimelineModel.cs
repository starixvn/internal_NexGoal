namespace NexGoal.Models.PerformanceRecord
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using NexGoal.Models.Common;

    public class MemberPerformanceTimelineModel : Employee
    {
        public IEnumerable<PerformanceRecord> PerformanceRecords { get; set; } = new List<PerformanceRecord>();

        public int TotalItems { get; set; }

        [JsonIgnore]
        public string GradeId { get; set; }
        public PerformanceGradeModel Grade { get; set; }
    }
}