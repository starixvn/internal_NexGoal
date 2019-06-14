namespace NexGoal.Models.PARecord
{
    using System;
    using System.Collections.Generic;
    using NexGoal.Models.Common;

    public class PARecordModel
    {
        public int PARecordId { get; set; }

        public int PeriodId { get; set; }
        public PeriodBaseModel Period { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int StatusId { get; set; }
        public string Status { get; set; }

        public string GradeId { get; set; }
        public PerformanceGrade Grade { get; set; }

        public string Comment { get; set; }

        public int ToeicPolicy { get; set; }

        public string OriginalGradeId { get; set; }
        public PerformanceGrade OriginalGrade { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsRevised { get; set; }

        public int? Toeic { get; set; }

        public DateTime? ToeicExpiredDate { get; set; }

        public IEnumerable<PAProcessModel> Processes { get; set; }
    }
}