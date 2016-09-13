namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetEOYQuestionCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "EOY_GetQuestion";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}