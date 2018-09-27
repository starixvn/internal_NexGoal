namespace NexGoal.Models.PARecord
{
    using System.Collections.Generic;
    using NexGoal.Models.Common;

    public class PARecordModel
    {
        public int PARecordId { get; set; }

        public int PeriodId { get; set; }
        public PeriodBaseModel Period { get; set; }

        public int EmployeeId { get; set; }
        public EmployeeBaseModel Employee { get; set; }

        public int StatusId { get; set; }
        public string Status { get; set; }

        public string GradeId { get; set; }
        public PerformanceGradeModel Grade { get; set; }

        public string Comment { get; set; }

        public IEnumerable<PAProcessModel> Processes { get; set; }
    }
}