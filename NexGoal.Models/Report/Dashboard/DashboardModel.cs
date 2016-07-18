namespace NexGoal.Model.Report.Dashboard
{
    using System.Collections.Generic;
    using NexGoal.Model.Employee;
    using NexGoal.Model.EmployeeEPA;

    public class DashboardModel
    {
        public EmployeeModel Employee { get; set; }

        public IEnumerable<EPAModel> MyEPAs { get; set; }

        public IEnumerable<EPAModel> MonthlyEPAs { get; set; }
    }
}