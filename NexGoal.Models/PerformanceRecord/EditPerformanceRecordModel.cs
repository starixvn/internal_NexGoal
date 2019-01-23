namespace NexGoal.Models.PerformanceRecord
{
    public class EditPerformanceRecordModel : CreatePerformanceRecordModel
    {
        public int PerformanceRecordId { get; set; }

        public bool IsFromDraft { get; set; }
    }
}