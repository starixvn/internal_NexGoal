namespace NexGoal.Model.Authentication
{
    using System.Collections.Generic;
    using NexGoal.Model.Employee;
    using NexGoal.Model.RoleManagement;

    public class UserSession
    {
        public UserSession()
        {
            Roles = new List<SystemRoleModel>();
            Functions = new List<FunctionModel>();
            Menus = new List<MenuGroupModel>();
        }

        public EmployeeModel EmployeeInfo { get; set; }

        public IEnumerable<SystemRoleModel> Roles { get; set; }

        public IEnumerable<FunctionModel> Functions { get; set; }

        public IEnumerable<MenuGroupModel> Menus { get; set; }
    }
}