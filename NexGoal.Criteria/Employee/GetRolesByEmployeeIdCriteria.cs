namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetRolesByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetRolesById";

        public override bool IsValid() => EmployeeId > 0;
    }
}