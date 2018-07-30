namespace NexGoal.Models.UserManagement
{
    public class EditUserRolesModel
    {
        public int EmployeeId { get; set; }

        public string SelectedRoleIds { get; set; }

        public int ModifiedBy { get; set; }
    }
}