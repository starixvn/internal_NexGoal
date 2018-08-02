namespace NexGoal.Criteria.Periods
{
    public class UpdatePeriodStatusCriteria : CommandBaseCriteria
    {
        public int PeriodId { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Period_UpdateStatus";

        public override bool IsValid() => PeriodId > 0 && ModifiedBy > 0;
    }
}