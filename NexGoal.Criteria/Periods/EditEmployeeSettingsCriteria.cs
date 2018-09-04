namespace NexGoal.Criteria.Periods
{
    public class EditEmployeeSettingsCriteria : CommandBaseCriteria
    {
        public string PARecordIds { get; set; }

        public string Settings { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Period_UpdateEmployeeSettings";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(PARecordIds) && !string.IsNullOrWhiteSpace(Settings) && ModifiedBy > 0;
    }
}