namespace NexGoal.AutoService.Criterias
{
    public class CreateNotificationCriteria : CommandBaseCriteria
    {
        public string Notifications { get; set; }

        public override string GetSettingKey() => "Notification_Insert";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Notifications);
    }
}