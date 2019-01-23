namespace NexGoal.Models.Notification
{
    using System;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Common;

    public class NotificationEntity
    {
        public int NotificationId { get; set; }

        public int CreatorId { get; set; }

        public Employee Creator { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public string Content { get; set; }

        public NotificationStatus StatusId { get; set; }

        public DateTime CreatedDate { get; set; }

        public NotificationActionType ActionTypeId { get; set; }
        public int? RecordId { get; set; }
    }
}