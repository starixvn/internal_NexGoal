namespace NexGoal.Criteria.Period
{
    using Fanex.Data.Repository;

    public class GetAllPeriodByReviewerCriteria : CriteriaBase
    {
        public int ReviewerId { get; set; }

        public override bool IsValid()
        {
            return ReviewerId > 0;
        }

        public override string GetSettingKey()
        {
            return "Period_GetAllByReviewer";
        }
    }
}