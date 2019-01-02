namespace NexGoal.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class MarkAsReadNotificationCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int StatusId { get; set; }

        public override string GetSettingKey() => "Notification_MarkAsRead";

        public override bool IsValid() => EmployeeId > 0;
    }
}