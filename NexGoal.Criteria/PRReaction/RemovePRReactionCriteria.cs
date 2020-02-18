namespace NexGoal.Criteria.PRReaction
{
    using NexGoal.Core.Criteria;

    public class RemovePRReactionCriteria : CommandBaseCriteria
    {
        public int PerformanceRecordId { get; set; }

        public int CreatorId { get; set; }

        public override string GetSettingKey() => "PRReaction_Delete";

        public override bool IsValid()
        {
            return PerformanceRecordId > 0 && CreatorId > 0;
        }
    }
}