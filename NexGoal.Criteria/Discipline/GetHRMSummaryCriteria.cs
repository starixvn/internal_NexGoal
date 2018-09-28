namespace NexGoal.Criteria.Discipline
{
    using System;
    using Fanex.Data.Repository;

    public class GetHRMSummaryCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Discipline_GetHRMSummary";

        public override bool IsValid() => EmployeeId > 0 &&
                                FromDate < ToDate;
    }
}