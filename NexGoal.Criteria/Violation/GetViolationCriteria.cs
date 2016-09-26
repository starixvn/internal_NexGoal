namespace NexGoal.Criteria.Violation
{
    using Fanex.Data.Repository;
    using System;

    public class GetViolationCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? IsActive { get; set; }

        public int? GroupId { get; set; }

        public int? EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "Violation_Get";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}