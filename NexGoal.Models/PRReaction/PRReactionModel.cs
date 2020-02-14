namespace NexGoal.Models.PRReaction
{
    using System;
    using Newtonsoft.Json;
    using NexGoal.Models.Common;

    public class PRReactionModel
    {
        [JsonIgnore]
        public int PRReactionId { get; set; }

        [JsonIgnore]
        public int PerformanceRecordId { get; set; }

        public Employee Creator { get; set; }

        public byte RoleId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}