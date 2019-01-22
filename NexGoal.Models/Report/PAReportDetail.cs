namespace NexGoal.Models.Report
{
    using System.Collections.Generic;
    using NexGoal.Models.Common;
    using NexGoal.Models.Discipline;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.PARecord;

    public class PAReportDetail
    {
        public EmployeeBaseModel Employee { get; set; }

        public IEnumerable<HRMSummaryModel> HRMSummaries { get; set; }

        public IEnumerable<DisciplineModel> Disciplines { get; set; }

        public IEnumerable<EmployeeNominationModel> Achievements { get; set; }

        public PARecordModel PARecord { get; set; }

        public IEnumerable<PerformanceGradeModel> AllGrades { get; set; }

        public string PreviousGradeId { get; set; }

        public PerformanceGradeModel PreviousGrade { get; set; }
    }
}