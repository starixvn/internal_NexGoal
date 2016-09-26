namespace NexGoal.Criteria.Violation
{
    using Fanex.Data.Repository;
    using System;

    public class GetViolationByPeriodIdCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "Violation_GetByPeriodId";
        }

        public override bool IsValid()
        {
            return PeriodId > 0 && EmployeeId > 0;
        }
    }
}