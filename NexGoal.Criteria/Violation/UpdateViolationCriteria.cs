namespace NexGoal.Criteria.Violation
{
    using Fanex.Data.Repository;
    using System;

    public class UpdateViolationCriteria : CriteriaBase
    {
        public int ViolationId { get; set; }

        public string Description { get; set; }

        public DateTime EffectedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Violation_Update";
        }

        public override bool IsValid()
        {
            return ViolationId > 0 && ModifiedBy > 0 && string.IsNullOrEmpty(Description);
        }
    }
}