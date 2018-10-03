namespace NexGoal.Models.Redmine
{
    using System.Collections.Generic;

    public class EmployeeWorkLoadModel
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public IList<Employee.EmployeeWorkLoadModel> Workloads { get; set; }
    }
}