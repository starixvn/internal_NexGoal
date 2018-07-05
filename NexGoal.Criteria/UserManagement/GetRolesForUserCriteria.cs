namespace NexGoal.Criteria.UserManagement
{
    using Fanex.Data.Repository;

    public class GetRolesForUserCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "User_GetRolesForUser";

        public override bool IsValid() => EmployeeId > 0;
    }
}