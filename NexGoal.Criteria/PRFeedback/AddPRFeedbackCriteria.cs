namespace NexGoal.Criteria.PRFeedback
{
    using NexGoal.Core.Criteria;

    public class AddPRFeedbackCriteria : CommandBaseCriteria
    {
        public int PerformanceRecordId { get; set; }

        public int CreatorId { get; set; }

        public string Content { get; set; }

        public override string GetSettingKey() => "PRFeedback_Insert";

        public override bool IsValid()
        {
            return !(string.IsNullOrEmpty(Content) || PerformanceRecordId == 0 || CreatorId == 0);
        }
    }
}