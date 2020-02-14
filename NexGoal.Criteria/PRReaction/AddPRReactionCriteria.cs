namespace NexGoal.Criteria.PRReaction
{
    using NexGoal.Core.Criteria;

    public class AddPRReactionCriteria : CommandBaseCriteria
    {
        public int PerformanceRecordId { get; set; }

        public int CreatorId { get; set; }

        public override string GetSettingKey() => "PRReaction_Insert";

        public override bool IsValid()
        {
            return PerformanceRecordId > 0 && CreatorId > 0;
        }
    }
}