namespace NexGoal.Models.PRFeedbackReaction
{
    using System;
    using NexGoal.Models.Common;

    public class PRFeedbackReactionModel
    {
        public int PRFeedbackId { get; set; }

        public Employee Creator { get; set; }

        public byte RoleId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
