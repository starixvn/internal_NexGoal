namespace NexGoal.Models.PAFocus
{
    using Newtonsoft.Json;
    using NexGoal.Models.Common;

    public class PAFocusModel
    {
        public int PAFocusId { get; set; }

        public EmployeeBaseModel Employee { get; set; } = new EmployeeBaseModel();

        public string CurrentObjective { get; set; }
        public string NextObjective { get; set; }
        public string Contribution { get; set; }
        public string BonusGiven { get; set; }
        public string IsReadyPromotion { get; set; }

        [JsonIgnore]
        public string GradeId { get; set; }
        public PerformanceGradeModel Grade { get; set; }

        public bool IsHost { get; set; }
    }
}