namespace NexGoal.Models.PRReaction
{
    public class AddPRFeedbackDto
    {
        public int PerformanceRecordId { get; set; }

        public int CreatorId { get; set; }

        public string Content { get; set; }
    }
}