namespace NexGoal.Models
{
    using Newtonsoft.Json;

    public class ProjectModel
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public int PerformanceRecordId { get; set; }
    }
}