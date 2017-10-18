namespace NexGoal.Model.Dashboard
{
    using System.Collections.Generic;
    using Employee;
    using EmployeeEPA;

    public class DashboardModel
    {
        public EmployeeModel Employee { get; set; }

        public IEnumerable<EPAModel> MyEPAs { get; set; }

        public ToDoListModel ToDoList { get; set; }
    }
}