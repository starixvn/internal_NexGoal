namespace NexGoal.Models.PerformanceRecord
{
    using System;

    public class GetPerformanceRecordByEmployeeParam
    {
        public int EmployeeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string CreatorIds { get; set; }
        public string ProjectIds { get; set; }
        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }
    }
}