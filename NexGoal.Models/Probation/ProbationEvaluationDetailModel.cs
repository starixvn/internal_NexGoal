namespace NexGoal.Models.Probation
{
    public class ProbationEvaluationDetailModel
    {
        public int EmployeeId { get; set; }

        public int KPIId { get; set; }

        public string KPIName { get; set; }

        public int KPIGroupId { get; set; }

        public int? SelfPoint { get; set; }

        public int? EvaluatorPoint { get; set; }
    }
}