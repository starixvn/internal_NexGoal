namespace NexGoal.Criteria.Periods
{
    using Fanex.Data.Repository;

    public class GetPeriodProcessSettingCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override string GetSettingKey() => "Period_GetProcessSetting";

        public override bool IsValid() => PeriodId > 0;
    }
}