namespace NexGoal.Criteria.Periods
{
    using Fanex.Data.Repository;

    public class GetPeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override string GetSettingKey() => "Period_GetById";

        public override bool IsValid() => PeriodId > 0;
    }
}