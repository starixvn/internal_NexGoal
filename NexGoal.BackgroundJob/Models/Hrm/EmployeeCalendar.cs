namespace NexGoal.BackgroundJob.Models.Hrm
{
    using System;

    public class EmployeeCalendar
    {
        public long Id { get; set; }
        public int EmpId { get; set; }
        public DateTime WorkDate { get; set; }
        public bool? IsOffDay { get; set; }
        public bool? IsHoliday { get; set; }
        public bool? IsLeaveDay { get; set; }
        public decimal? LeaveDay { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? SyncDate { get; set; }
    }
} 