namespace NexGoal.Criteria.Periods
{
    using NexGoal.Core.Criteria;

    public class EditProcessSettingCriteria : CommandBaseCriteria
    {
        public int PeriodId { get; set; }

        public string Settings { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Period_UpdateProcessSetting";

        public override bool IsValid() => PeriodId > 0 && ModifiedBy > 0 && !string.IsNullOrWhiteSpace(Settings);
    }
}