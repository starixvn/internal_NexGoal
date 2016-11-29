namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetEOQYToSendEmailWhenRemindCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "EOQY_GetToSendEmailWhenReminding";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}