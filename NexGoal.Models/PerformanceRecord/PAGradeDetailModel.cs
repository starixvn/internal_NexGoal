namespace NexGoal.Models.PerformanceRecord
{
    public class PAGradeDetailModel
    {
        public int PARecordId { get; set; }

        public int ProcessId { get; set; }

        public string Process { get; set; }

        public string GradeId { get; set; }

        public PerformanceGrade Grade { get; set; }
    }
}