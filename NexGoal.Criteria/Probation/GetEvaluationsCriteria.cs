namespace NexGoal.Criteria.Probation
{
    using Fanex.Data.Repository;

    public class GetEvaluationsCriteria : CriteriaBase
    {
        public int? EmployeeId { get; set; }

        public int ActorId { get; set; }

        public override string GetSettingKey() => "Probation_GetEvaluations";

        public override bool IsValid() => ActorId > 0;
    }
}