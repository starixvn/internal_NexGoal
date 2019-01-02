namespace NexGoal.Models.Notification
{
    using System;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Common;

    public class Notification
    {
        public int NotificationId { get; set; }

        public int CreatorId { get; set; }

        public EmployeeBaseModel Creator { get; set; }

        public int EmployeeId { get; set; }

        public EmployeeBaseModel Employee { get; set; }

        public string Content { get; set; }

        public NotificationStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}