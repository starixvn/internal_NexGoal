namespace NexGoal.Core.Criteria
{
    using Fanex.Data.Repository;

    public class InsertLogLoginCriteria : CriteriaBase
    {
        public string WindowsUser { get; set; }

        public string IdentifyUser { get; set; }

        public string LoginUser { get; set; }

        public string IPAddress { get; set; }
        
        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(WindowsUser)
                && !string.IsNullOrWhiteSpace(IdentifyUser)
                && !string.IsNullOrWhiteSpace(LoginUser)
                && !string.IsNullOrWhiteSpace(IPAddress);
        }

        public override string GetSettingKey()
        {
            return "LogLogin_Insert";
        }
    }
}