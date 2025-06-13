namespace NexGoal.BackgroundJob.Models.Hrm
{
    using System;

    public class EmployeeOvertime
    {
        public int OvertimeId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public decimal Hour { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? SyncDate { get; set; }
    }
} 