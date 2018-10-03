namespace NexGoal.Models.Redmine
{
    using System.Collections.Generic;
    using NexGoal.Models.Employee;

    public class EmployeeWorkLoadModel
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public IList<ProjectWorkloadModel> Workloads { get; set; }
    }
}