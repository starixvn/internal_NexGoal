namespace NexGoal.Criteria.PAFocus
{
    using Fanex.Data.Repository;

    public class GetPAFocusCriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public string PeriodIds { get; set; }

        public string GroupIds { get; set; }

        public int? EmployeeId { get; set; }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }

        public override string GetSettingKey()
        {
            return "PAFocus_Get";
        }
    }
}