namespace NexGoal.Criteria.Probation
{
    using NexGoal.Core.Criteria;

    public class UpdateSettingCriteria : CommandBaseCriteria
    {
        public string Settings { get; set; }

        public override string GetSettingKey() => "Probation_UpdateSetting";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Settings);
    }
}