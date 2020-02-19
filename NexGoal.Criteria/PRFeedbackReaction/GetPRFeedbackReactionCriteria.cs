namespace NexGoal.Criteria.PRFeedbackReaction
{
    using Fanex.Data.Repository;

    public class GetPRFeedbackReactionCriteria : CriteriaBase
    {
        public int PRFeedbackId { get; set; }

        public int ActorId { get; set; }

        public override string GetSettingKey() => "PRFeedbackReaction_GetByFeedbackId";

        public override bool IsValid()
        {
            return PRFeedbackId > 0 && ActorId > 0;
        }
    }
}