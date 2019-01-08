namespace NexGoal.Models.PerformanceRecord
{
    using NexGoal.Core.Enumerations;

    public class ProcessSettingModel
    {
        public int PerformanceRecordId { get; set; }
        public PerformanceRecordMailMode NotificationMode { get; set; }
        public int Priority { get; set; }
        public int EvaluatorId { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
    }
}