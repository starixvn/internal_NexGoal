namespace NexGoal.BackgroundJob.Models.Hrm
{
    using System;

    public class JobTitle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? SyncDate { get; set; }
    }
} 