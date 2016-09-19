namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetYearEOQCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "EOQ_GetYear";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}