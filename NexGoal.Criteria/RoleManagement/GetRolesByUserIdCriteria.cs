namespace NexGoal.Criteria.RoleManagement
{
    using Fanex.Data.Repository;

    public class GetRolesByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "Role_GetRolesByEmployeeId";
        }
    }
}
