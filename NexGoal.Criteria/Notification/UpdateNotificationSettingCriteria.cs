namespace NexGoal.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class UpdateNotificationSettingCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string Value { get; set; }

        public override string GetSettingKey() => "Notification_UpdateSetting";

        public override bool IsValid() => EmployeeId > 0 && !string.IsNullOrWhiteSpace(Value);
    }
}