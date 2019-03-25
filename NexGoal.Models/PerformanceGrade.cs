namespace NexGoal.Models
{
    using Newtonsoft.Json;

    public class PerformanceGrade
    {
        public int GradeId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Explanation { get; set; }

        [JsonIgnore]
        public float Point { get; set; }
    }
}