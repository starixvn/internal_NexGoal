namespace NexGoal.Models.PerformanceRecord
{
    public class ProcessSettingModel
    {
        public int PerformanceRecordId { get; set; }
        public int ProcessId { get; set; }
        public int EvaluatorId { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
    }
}