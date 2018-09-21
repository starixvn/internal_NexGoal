namespace NexGoal.Models.PARecord
{
    using System;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.UserManagement;

    public class PAProcessModel
    {
        public int PARecordId { get; set; }

        public int ProcessId { get; set; }
        public string Process { get; set; }

        public int EvaluatorId { get; set; }
        public UserModel Evaluator { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? StatusId { get; set; }
        public string Status { get; set; }

        public int PerformanceRecordId { get; set; }
        public PerformanceRecordModel PerformanceRecord { get; set; }
    }
}