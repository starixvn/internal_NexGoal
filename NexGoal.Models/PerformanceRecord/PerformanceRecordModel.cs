namespace NexGoal.Models.PerformanceRecord
{
    using System;
    using NexGoal.Core.Enumerations;

    public class PerformanceRecordModel
    {
        public int PerformanceRecordId { get; set; }
        public string Grade { get; set; }
        public string Comment { get; set; }
        public PerformanceRecordStatus StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}