namespace NexGoal.Models.Redmine
{
    using System.Collections.Generic;

    public class TeamWorkloadModel
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public IList<EmployeeWorkloadModel> EmployeeWorkloads { get; set; }
    }
}