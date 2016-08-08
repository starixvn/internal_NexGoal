namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class GetHRMSummaryByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public override string GetSettingKey()
        {
            return "HRMSummary_GetByEmployeeId";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0
                && StartDate != DateTime.MaxValue
                && StartDate != DateTime.MinValue
                && EndDate != DateTime.MinValue
                && EndDate != DateTime.MaxValue
                && EndDate >= StartDate;
        }
    }
}
