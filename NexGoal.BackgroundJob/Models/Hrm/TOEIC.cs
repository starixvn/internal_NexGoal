namespace NexGoal.BackgroundJob.Models.Hrm
{
    using System;

    public class TOEIC
    {
        public int EmpId { get; set; }
        public int? MinimumScore { get; set; }
        public int? Listening { get; set; }
        public int? Reading { get; set; }
        public DateTime? LRExamDate { get; set; }
        public DateTime? InputDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? SyncDate { get; set; }
    }
} 