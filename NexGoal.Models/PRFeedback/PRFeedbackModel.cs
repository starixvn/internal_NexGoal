namespace NexGoal.Models.PRFeedback
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using NexGoal.Models.Common;
    using NexGoal.Models.PRFeedbackReaction;

    public class PRFeedbackModel
    {
        public int PRFeedbackId { get; set; }

        [JsonIgnore]
        public int PerformanceRecordId { get; set; }

        public Employee Creator { get; set; }

        public byte RoleId { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public IEnumerable<PRFeedbackReactionModel> Reactions { get; set; } = new List<PRFeedbackReactionModel>();
    }
}