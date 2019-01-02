namespace NexGoal.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class UpdateNotificationStatusCriteria : CriteriaBase
    {
        public int NotificationId { get; set; }

        public int EmployeeId { get; set; }

        public int StatusId { get; set; }

        public override string GetSettingKey() => "Notification_UpdateStatus";

        public override bool IsValid() => NotificationId > 0 && EmployeeId > 0;
    }
}