namespace NexGoal.Models.Report
{
    using System.Collections.Generic;
    using NexGoal.Models.Common;
    using NexGoal.Models.Discipline;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.PARecord;

    public class PAReportDetail
    {
        public Employee Employee { get; set; }

        public IEnumerable<HRMInfo> HRM { get; set; }

        public IEnumerable<DisciplineModel> Disciplines { get; set; }

        public IEnumerable<EmployeeNominationModel> Achievements { get; set; }

        public PARecordModel PARecord { get; set; }

        public IEnumerable<PerformanceGrade> AllGrades { get; set; }

        public string PreviousGradeId { get; set; }

        public PerformanceGrade PreviousGrade { get; set; }
    }
}