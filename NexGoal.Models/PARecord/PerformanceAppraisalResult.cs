namespace NexGoal.Models.PARecord
{
    using Newtonsoft.Json;
    using NexGoal.Models.Common;

    public class PerformanceAppraisalResult
    {
        public int PARecordId { get; set; }

        public Employee Employee { get; set; }

        public PeriodBaseModel Period { get; set; }

        [JsonIgnore]
        public string GradeId { get; set; }
        public PerformanceGrade Grade { get; set; }
    }
}