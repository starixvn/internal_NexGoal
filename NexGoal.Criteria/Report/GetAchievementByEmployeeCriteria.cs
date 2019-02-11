namespace NexGoal.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetAchievementByEmployeeCriteria : CriteriaBase
    {
        public int PARecordId { get; set; }

        public override string GetSettingKey() => "Nomination_GetByEmployeeInPeriod";

        public override bool IsValid() => PARecordId > 0;
    }
}