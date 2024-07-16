namespace NexGoal.BackgroundJob.Models.Redmine
{
    using System;

    public class TimeEntries
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public int UserId { get; set; }

        public int IssueId { get; set; }

        public float Hours { get; set; }

        public DateTime SpentOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
