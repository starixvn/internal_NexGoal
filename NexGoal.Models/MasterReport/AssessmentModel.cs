namespace NexGoal.Models.MasterReport
{
    using NexGoal.Core.Enumerations;

    public class AssessmentModel
    {
        public int EvaluatorId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string GradeId { get; set; }

        public string Grade { get; set; }

        public string Comment { get; set; }

        public PerformanceProcess ProcessId { get; set; }

        public int EmployeeId { get; set; }

        public int? PerformanceRecordId { get; set; }
    }
}