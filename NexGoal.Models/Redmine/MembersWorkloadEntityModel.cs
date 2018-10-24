namespace NexGoal.Models.Redmine
{
    public class MembersWorkloadEntityModel
    {
        public int EmployeeId { get; set; }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string NickName { get; set; }

        public float TotalHour { get; set; }

        public int ProjectId { get; set; }

        public string ProjectName { get; set; }
    }
}