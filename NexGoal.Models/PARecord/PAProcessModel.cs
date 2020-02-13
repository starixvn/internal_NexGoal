namespace NexGoal.Models.PARecord
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord;

    public class PAProcessModel
    {
        public int PARecordId { get; set; }

        public byte ProcessId { get; set; }
        public string Process { get; set; }

        public int EvaluatorId { get; set; }
        public Employee Evaluator { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? StatusId { get; set; }
        public string Status { get; set; }

        [JsonIgnore]
        public string GradeId { get; set; }
        public string Grade { get; set; }

        [JsonIgnore]
        public string RelatedPerformanceRecordIds { get; set; }
        public IEnumerable<PerformanceRecord> RelatedPerformanceRecords { get; set; } = new List<PerformanceRecord>();
    }
}