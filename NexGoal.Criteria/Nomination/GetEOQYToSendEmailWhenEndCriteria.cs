namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetEOQYToSendEmailWhenEndCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "EOQY_GetToSendEmailWhenEnd";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}