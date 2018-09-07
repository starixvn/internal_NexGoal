namespace NexGoal.Criteria.Periods
{
    public class CreateEmployeeSettingsCriteria : CommandBaseCriteria
    {
        public int PeriodId { get; set; }

        public string GroupIds { get; set; }

        public int? EmployeeId { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Period_InsertEmployeeSettings";

        public override bool IsValid() => PeriodId > 0 && CreatedBy > 0 && (!string.IsNullOrWhiteSpace(GroupIds) || EmployeeId.HasValue);
    }
}