namespace NexGoal.Criteria.Violation
{
    using Fanex.Data.Repository;

    public class UpdateViolationActiveCriteria : CriteriaBase
    {
        public int ViolationId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Violation_UpdateActive";
        }

        public override bool IsValid()
        {
            return ViolationId > 0 && ModifiedBy > 0;
        }
    }
}