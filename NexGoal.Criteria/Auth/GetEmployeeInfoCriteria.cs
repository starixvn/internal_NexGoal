namespace NexGoal.Criteria.Auth
{
    using Fanex.Data.Repository;

    public class GetEmployeeInfoCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int UnreadNotification { get; set; }

        public bool IsHasMember { get; set; }

        public override string GetSettingKey() => "Employee_GetInfo";

        public override bool IsValid() => EmployeeId > 0;
    }
}