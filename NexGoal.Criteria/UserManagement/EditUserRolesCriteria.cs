namespace NexGoal.Criteria.UserManagement
{
    public class EditUserRolesCriteria : CommandBaseCriteria
    {
        public int EmployeeId { get; set; }

        public int ModifiedBy { get; set; }

        public string SelectedRoleIds { get; set; }

        public override string GetSettingKey() => "User_EditRoles";

        public override bool IsValid() => EmployeeId > 0 && ModifiedBy > 0;
    }
}