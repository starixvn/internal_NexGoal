namespace NexGoal.BackgroundJob.Criteria
{
    using NexGoal.Core.Criteria;

    public class CreateNotificationCriteria : CommandBaseCriteria
    {
        public string Notifications { get; set; }

        public override string GetSettingKey() => "Auto_Notification_Insert";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Notifications);
    }
}