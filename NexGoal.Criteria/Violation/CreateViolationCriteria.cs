namespace NexGoal.Criteria.Violation
{
    using Fanex.Data.Repository;
    using System;

    public class CreateViolationCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string Description { get; set; }

        public DateTime EffectedDate { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Violation_Insert";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0 && CreatedBy > 0 && string.IsNullOrEmpty(Description);
        }
    }
}