namespace NexGoal.Models.PerformanceRecord
{
    public class EditPerformanceRecordDto : CreatePerformanceRecordDto
    {
        public int PerformanceRecordId { get; set; }

        public bool IsFromDraft { get; set; }
    }
}