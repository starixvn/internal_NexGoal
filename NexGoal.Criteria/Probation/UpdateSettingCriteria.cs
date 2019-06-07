namespace NexGoal.Criteria.Probation
{
    using NexGoal.Core.Criteria;

    public class UpdateSettingCriteria : CommandBaseCriteria
    {
        public string Settings { get; set; }

        public int ActorId { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Probation_UpdateSetting";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Settings) && ActorId > 0 && EmployeeId > 0;
    }
}