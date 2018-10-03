namespace NexGoal.Models.Redmine
{
    using System.Collections.Generic;

    public class EmployeeWorkloadModel
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public IList<ProjectWorkloadModel> Workloads { get; set; }
    }
}