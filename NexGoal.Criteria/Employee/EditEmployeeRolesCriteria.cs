namespace NexGoal.Criteria.Employee
{
    public class EditEmployeeRolesCriteria : CommandBaseCriteria
    {
        public int EmployeeId { get; set; }

        public int ModifiedBy { get; set; }

        public string SelectedRoleIds { get; set; }

        public override string GetSettingKey() => "Employee_UpdateRoles";

        public override bool IsValid() => EmployeeId > 0 && ModifiedBy > 0;
    }
}