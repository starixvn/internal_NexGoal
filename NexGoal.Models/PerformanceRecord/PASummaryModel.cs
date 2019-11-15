namespace NexGoal.Models.PerformanceRecord
{
    using System.Collections.Generic;

    public class PASummaryModel
    {
        public int PARecordId { get; set; }

        public int PerformanceRecordId { get; set; }

        public string OriginalGradeId { get; set; }

        public PerformanceGrade OriginalGrade { get; set; }

        public int ToeicPolicy { get; set; }

        public int? Toeic { get; set; }

        public IEnumerable<PAGradeDetailModel> GradeDetails { get; set; }
    }
}