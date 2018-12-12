namespace NexGoal.Criteria.Periods
{
    public class EditEmployeeSettingStatusCriteria : CommandBaseCriteria
    {
        public int PARecordId { get; set; }

        public bool IsExcluded { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Period_UpdateEmployeeSettingStatus";

        public override bool IsValid() => PARecordId > 0 && ModifiedBy > 0;
    }
}