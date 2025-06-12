namespace NexGoal.BackgroundJob.Models.Hrm
{
    using System;

    public class EmployeeAttendance
    {
        public long Id { get; set; }
        public int EmpId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime? InFixed { get; set; }
        public DateTime? OutFixed { get; set; }
        public int? LeaveTypeId { get; set; }
        public decimal? TotalWorkHours { get; set; }
        public decimal? LeaveDay { get; set; }
        public int? LeaveTypeId2 { get; set; }
        public DateTime? InsertDate { get; set; }
    }
} 