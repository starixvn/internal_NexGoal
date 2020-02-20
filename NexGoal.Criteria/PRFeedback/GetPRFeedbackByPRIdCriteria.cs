namespace NexGoal.Criteria.PRReaction
{
    using Fanex.Data.Repository;

    public class GetPRFeedbackByPRIdCriteria : CriteriaBase
    {
        public int PerformanceRecordId { get; set; }

        public int ActorID { get; set; }

        public override string GetSettingKey() => "PRFeedback_GetByPRId";

        public override bool IsValid()
        {
            return PerformanceRecordId > 0 && ActorID > 0;
        }
    }
}