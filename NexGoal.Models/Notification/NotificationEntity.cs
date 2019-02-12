namespace NexGoal.Models.Notification
{
    using System;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;

    public class NotificationEntity
    {
        public int NotificationId { get; set; }

        public int CreatorId { get; set; }

        public string CreatorCode { get; set; }

        public string CreatorAvatar
        {
            get
            {
                return GetFilePath(CreatorId, CreatorCode);
            }
        }

        public int EmployeeId { get; set; }

        public string Content { get; set; }

        public NotificationStatus StatusId { get; set; }

        public DateTime CreatedDate { get; set; }

        public NotificationActionType ActionTypeId { get; set; }

        public int? RecordId { get; set; }

        private static string GetFilePath(int employeeId, string code)
        {
            var imgUrl = employeeId == Constants.NexGoalSystemId ? "/dist/avatars/nexgoal.jpg" : new Uri($"{Constants.ImageURL}/{code}.jpg").ToString();
            return imgUrl;
        }
    }
}