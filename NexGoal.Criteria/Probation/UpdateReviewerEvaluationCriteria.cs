namespace NexGoal.Criteria.Probation
{
    using NexGoal.Core.Criteria;

    public class UpdateReviewerEvaluationCriteria : CommandBaseCriteria
    {
        public int FinalScore { get; set; }

        public int EmployeeId { get; set; }
        public string Comment { get; set; }

        public override string GetSettingKey() => "Probation_UpdateReviewerEvaluation";

        public override bool IsValid() => FinalScore > 0 && EmployeeId > 0;
    }
}