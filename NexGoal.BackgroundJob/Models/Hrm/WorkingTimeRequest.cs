namespace NexGoal.BackgroundJob.Models.Hrm
{
    using System;

    public class WorkingTimeRequest
    {
        public long Id { get; set; }
        public int EmpId { get; set; }
        public short RequestType { get; set; }
        public byte Status { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? TotalDays { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
} 