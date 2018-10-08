namespace NexGoal.Models.PAFocus
{
    using Newtonsoft.Json;
    using NexGoal.Models.Common;

    public class PAFocusModel
    {
        public int PAFocusId { get; set; }

        [JsonIgnore]
        public int EmployeeId { get; set; }
        public EmployeeBaseModel Employee { get; set; }

        public string LastCompletedPercentObjective { get; set; }
        public string NextDescriptionObjective { get; set; }
        public string Contribution { get; set; }
        public string BonusGiven { get; set; }
        public string IsReadyPromotion { get; set; }
        public bool IsHost { get; set; }
    }
}