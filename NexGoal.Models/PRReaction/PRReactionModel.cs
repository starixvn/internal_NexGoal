namespace NexGoal.Models.PRReaction
{
    using System;

    public class PRReactionModel
    {
        public int PRReactionId { get; set; }

        public int PerformanceRecordId { get; set; }

        public int CreatorId { get; set; }

        public int RoleId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}