namespace NexGoal.Criteria.Violation
{
    using Fanex.Data.Repository;

    public class GetViolationByIdCriteria : CriteriaBase
    {
        public int ViolationId { get; set; }

        public override string GetSettingKey()
        {
            return "Violation_GetById";
        }

        public override bool IsValid()
        {
            return ViolationId > 0;
        }
    }
}