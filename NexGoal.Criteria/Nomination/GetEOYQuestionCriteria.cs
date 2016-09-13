namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetEOYQuestionCriteria : CriteriaBase
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