namespace NexGoal.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class GetNotificationByEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int UnReadNoti { get; set; }

        public int TotalItems { get; set; }

        public override string GetSettingKey() => "Notification_GetByEmployee";

        public override bool IsValid() => PageSize > 0 && EmployeeId > 0;
    }
}