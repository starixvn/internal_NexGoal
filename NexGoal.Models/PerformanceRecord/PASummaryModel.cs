namespace NexGoal.Models.PerformanceRecord
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class PASummaryModel
    {
       // [JsonIgnore]
        public int PARecordId { get; set; }
        [JsonIgnore]
        public int PerformanceRecordId { get; set; }

        [JsonIgnore]
        public string OriginalGradeId { get; set; }
        public PerformanceGrade OriginalGrade { get; set; }

        public int ToeicPolicy { get; set; }
        public int? Toeic { get; set; }

        public IEnumerable<PAGradeDetailModel> GradeDetails { get; set; } = new List<PAGradeDetailModel>();
    }
}