namespace NexGoal.Model.Dashboard
{
    using NexGoal.Model.Employee;
    using NexGoal.Model.EmployeeEPA;
    using System.Collections.Generic;

    public class DashboardModel
    {
        public EmployeeModel Employee { get; set; }

        public IEnumerable<EPAModel> MyEPAs { get; set; }

        public ToDoListModel ToDoList { get; set; }
    }
}