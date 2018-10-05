namespace NexGoal.Models.Redmine
{
    using System.Collections.Generic;

    public class TeamWorkloadModel
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public IEnumerable<EmployeeWorkloadModel> EmployeeWorkloads { get; set; }
    }
}