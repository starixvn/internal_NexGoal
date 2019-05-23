namespace NexGoal.Criteria.Probation
{
    using Fanex.Data.Repository;

    public class GetEvaluationDetailCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Probation_GetEvaluationDetail";

        public override bool IsValid() => EmployeeId > 0;
    }
}