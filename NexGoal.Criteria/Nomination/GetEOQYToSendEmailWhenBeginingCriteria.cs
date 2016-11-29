namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetEOQYToSendEmailWhenBeginingCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "EOQY_GetToSendEmailWhenBegining";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}