namespace NexGoal.Models.PerformanceRecord
{
    using Newtonsoft.Json;

    public class PAGradeDetailModel
    {
        [JsonIgnore]
        public int PARecordId { get; set; }

        public byte ProcessId { get; set; }
        public string Process { get; set; }

        [JsonIgnore]
        public string GradeId { get; set; }
        public string Grade { get; set; }

        [JsonIgnore]
        public string RelatedPerformanceRecordIds { get; set; }
    }
}