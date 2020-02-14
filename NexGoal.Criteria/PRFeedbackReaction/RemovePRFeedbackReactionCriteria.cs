namespace NexGoal.Criteria.PRFeedbackReaction
{
    using NexGoal.Core.Criteria;

    public class RemovePRFeedbackReactionCriteria : CommandBaseCriteria
    {
        public int PRFeedbackReactionId { get; set; }

        public int CreatorId { get; set; }

        public override string GetSettingKey() => "PRFeedbackReation_Delete";

        public override bool IsValid()
        {
            return PRFeedbackReactionId > 0 && CreatorId > 0;
        }
    }
}