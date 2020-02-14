namespace NexGoal.Models.PRFeedbackReaction
{
    using System;

    public class PRFeedbackReactionModel
    {
        public int PRFeedbackId { get; set; }

        public int CreatorId { get; set; }

        public byte RoleId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
