namespace NexGoal.Tools
{
    using Fanex.Data.Repository;

    public class GetEPAInfoToChangeCertificateCriteria : CriteriaBase
    {
        public int EPAIdFrom { get; set; }

        public int EPAIdTo { get; set; }

        public override string GetSettingKey()
        {
            return "NexGoalTools_GetEPAInfoToChangeCertificate";
        }

        public override bool IsValid()
        {
            return EPAIdTo >= EPAIdFrom;
        }
    }
}