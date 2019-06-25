namespace NexGoal.Criteria.Probation
{
    using NexGoal.Core.Criteria;

    public class UpdateReviewerEvaluationCriteria : CommandBaseCriteria
    {
        public int EmployeeId { get; set; }

        public string Comment { get; set; }

        public bool IsQualified { get; set; }

        public int ActorId { get; set; }

        public override string GetSettingKey() => "Probation_UpdateReviewerEvaluation";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Comment) && EmployeeId > 0 && ActorId > 0;
    }
}