namespace NexGoal.Criteria.Probation
{
    using NexGoal.Core.Criteria;

    public class UpdateSelfEvaluationCriteria : CommandBaseCriteria
    {
        public string Details { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Probation_UpdateSelfEvaluation";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Details) && EmployeeId > 0;
    }
}