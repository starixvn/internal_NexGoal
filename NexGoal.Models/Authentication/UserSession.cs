namespace NexGoal.Model.Authentication
{
    using System.Collections.Generic;
    using NexGoal.Model.Employee;
    using NexGoal.Model.RoleManagement;

    public class UserSession
    {
        public UserSession()
        {
            Functions = new List<FunctionModel>();
        }

        public EmployeeModel EmployeeInfo { get; set; }

        public IEnumerable<FunctionModel> Functions { get; set; }
    }
}