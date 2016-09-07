namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetEOQQuestionCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "EOQ_GetQuestion";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}