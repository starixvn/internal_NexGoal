namespace NexGoal.Criteria.PRReaction
{
    using NexGoal.Core.Criteria;

    public class RemovePRReactionCriteria : CommandBaseCriteria
    {
        public int PRReactionId { get; set; }

        public int CreatorId { get; set; }

        public override string GetSettingKey() => "PRReaction_Delete";

        public override bool IsValid()
        {
            return PRReactionId > 0 && CreatorId > 0;
        }
    }
}