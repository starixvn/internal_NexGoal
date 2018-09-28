namespace NexGoal.Models.Employee
{
    public class EditEmployeeRolesModel
    {
        public int EmployeeId { get; set; }

        public string SelectedRoleIds { get; set; }

        public int ModifiedBy { get; set; }
    }
}