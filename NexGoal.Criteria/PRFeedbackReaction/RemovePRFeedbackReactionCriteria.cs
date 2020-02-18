namespace NexGoal.Criteria.PRFeedbackReaction
{
    using NexGoal.Core.Criteria;

    public class RemovePRFeedbackReactionCriteria : CommandBaseCriteria
    {
        public int PRFeedbackId { get; set; }

        public int CreatorId { get; set; }

        public override string GetSettingKey() => "PRFeedbackReation_Delete";

        public override bool IsValid()
        {
            return PRFeedbackId > 0 && CreatorId > 0;
        }
    }
}