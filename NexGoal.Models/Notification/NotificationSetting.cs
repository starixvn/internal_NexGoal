namespace NexGoal.Models.Notification
{
    using NexGoal.Core.Enumerations;

    public class NotificationSetting
    {
        public NotificationType Type { get; set; }

        public string Description { get; set; }

        public bool ReceiveNotification { get; set; }

        public bool ReceiveEmail { get; set; }
    }
}