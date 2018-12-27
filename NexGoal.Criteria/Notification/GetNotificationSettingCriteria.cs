namespace NexGoal.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class GetNotificationSettingCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Notification_GetSettingByEmployee";

        public override bool IsValid() => EmployeeId > 0;
    };
}