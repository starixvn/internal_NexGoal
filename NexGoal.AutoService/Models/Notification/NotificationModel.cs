namespace NexGoal.AutoService.Models.Notification
{
    using System;
    using NexGoal.Core.Extensions;

    [Serializable]
    [XMLName("Row")]
    public class NotificationModel
    {
        public int EmployeeId { get; set; }
        public string Content { get; set; }
        public int StatusId { get; set; }
    }
}