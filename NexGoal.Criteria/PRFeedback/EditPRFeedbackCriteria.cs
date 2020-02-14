namespace NexGoal.Criteria.PRFeedback
{
    using NexGoal.Core.Criteria;

    public class EditPRFeedbackCriteria : CommandBaseCriteria
    {
        public int PRFeedbackId { get; set; }

        public int CreatorId { get; set; }

        public string Content { get; set; }

        public override string GetSettingKey() => "PRFeedback_Update";

        public override bool IsValid()
        {
            return !(string.IsNullOrEmpty(Content) || PRFeedbackId <= 0 || CreatorId <= 0);
        }
    }
}