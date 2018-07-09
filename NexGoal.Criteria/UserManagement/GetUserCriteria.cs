namespace NexGoal.Criteria.UserManagement
{
    using Fanex.Data.Repository;

    public class GetUserCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "User_GetById";

        public override bool IsValid() => EmployeeId > 0;
    }
}