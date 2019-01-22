namespace NexGoal.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetAchievementByEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int PeriodId { get; set; }

        public override string GetSettingKey() => "Nomination_GetByEmployeeInPeriod";

        public override bool IsValid() => EmployeeId > 0 && PeriodId > 0;
    }
}