namespace NexGoal.Tools
{
    using Fanex.Data.Repository;

    public class GetEPAInfoToChangeCertificateCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "NexGoalTools_GetEPAInfoToChangeCertificate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}