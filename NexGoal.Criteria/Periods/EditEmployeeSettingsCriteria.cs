namespace NexGoal.Criteria.Periods
{
    using NexGoal.Core.Criteria;

    public class EditEmployeeSettingsCriteria : CommandBaseCriteria
    {
        public int PeriodId { get; set; }

        public string PARecordIds { get; set; }

        public string Settings { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Period_UpdateEmployeeSettings";

        public override bool IsValid() => PeriodId > 0 && !string.IsNullOrWhiteSpace(PARecordIds) && !string.IsNullOrWhiteSpace(Settings) && ModifiedBy > 0;
    }
}