namespace NexGoal.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class GetNotificationByEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Notification_GetByEmployee";

        public override bool IsValid() => EmployeeId > 0;
    }
}