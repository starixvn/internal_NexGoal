namespace NexGoal.Models.Probation
{
    public class EncryptedProbationEvaluationDetailModel
    {
        public int EmployeeId { get; set; }

        public int KPIId { get; set; }

        public string KPIName { get; set; }

        public int KPIGroupId { get; set; }

        public string SelfPoint { get; set; }

        public string EvaluatorPoint { get; set; }
    }
}