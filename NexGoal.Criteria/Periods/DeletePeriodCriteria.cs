namespace NexGoal.Criteria.Periods
{
    public class DeletePeriodCriteria : CommandBaseCriteria
    {
        public int PeriodId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Period_Delete";

        public override bool IsValid() => PeriodId > 0 && ModifiedBy > 0;
    }
}