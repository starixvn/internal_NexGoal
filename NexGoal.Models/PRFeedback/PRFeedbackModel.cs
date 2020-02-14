namespace NexGoal.Models.PRReaction
{
    using System;

    public class PRFeedbackModel
    {
        public int PRFeedbackId { get; set; }

        public int PerformanceRecordId { get; set; }

        public int CreatorId { get; set; }

        public byte RoleId { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}