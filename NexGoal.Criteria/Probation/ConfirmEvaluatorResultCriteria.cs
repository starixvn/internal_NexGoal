namespace NexGoal.Criteria.Probation
{
    using NexGoal.Core.Criteria;

    public class ConfirmEvaluatorResultCriteria : CommandBaseCriteria
    {
        public int EmployeeId { get; set; }

        public int ActorId { get; set; }

        public override string GetSettingKey() => "Probation_ConfirmEvaluatorResult";

        public override bool IsValid() => EmployeeId > 0 && ActorId > 0;
    }
}