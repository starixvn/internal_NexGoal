namespace NexGoal.BackgroundJob.Models.Redmine
{
    using System;

    public class User
    {
        public int Id { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string Login { get; set; }
    }
}
