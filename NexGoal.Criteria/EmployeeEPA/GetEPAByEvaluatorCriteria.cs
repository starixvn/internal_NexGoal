namespace NexGoal.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEPAByEvaluatorCriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public string EmployeeIds { get; set; }

        public int? GroupId { get; set; }

        public string PeriodIds { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_GetByEvaluator";
        }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }
    }
}