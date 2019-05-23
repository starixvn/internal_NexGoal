namespace NexGoal.Criteria.Probation
{
    using NexGoal.Core.Criteria;

    public class UpdateEvaluatorEvaluationCriteria : CommandBaseCriteria
    {
        public string Details { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Probation_UpdateEvaluatorEvaluation";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Details) && EmployeeId > 0;
    }
}