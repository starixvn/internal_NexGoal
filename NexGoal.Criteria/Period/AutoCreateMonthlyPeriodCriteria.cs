namespace NexGoal.Criteria.Period
{
    using Fanex.Data.Repository;
    using NexGoal.Core.Attribute;
    using System;

    public class AutoCreateMonthlyPeriodCriteria : CriteriaBase
    {
        [Output]
        public int PeriodId { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_Monthly_InsertPeriod";
        }
    }
}