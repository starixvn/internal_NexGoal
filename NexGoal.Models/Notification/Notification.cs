namespace NexGoal.Models.Notification
{
    using System.Collections.Generic;

    public class Notification
    {
        public IEnumerable<NotificationEntity> Notifications { get; set; }

        public int UnreadNoti { get; set; }

        public int Total { get; set; }
    }
}