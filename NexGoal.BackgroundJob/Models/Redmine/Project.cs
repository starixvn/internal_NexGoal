namespace NexGoal.BackgroundJob.Models.Redmine
{
    public class Project
    {
        public int Id { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }
    }
}
