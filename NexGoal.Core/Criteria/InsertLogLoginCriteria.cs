namespace NexGoal.Core.Criteria
{
    using Fanex.Data.Repository;

    public class InsertLogLoginCriteria : CriteriaBase
    {
        public string IdentifyUser { get; set; }

        public string LoginUser { get; set; }

        public string MachineName { get; set; }

        public string IPAddress { get; set; }

        public string BrowserName { get; set; }
        
        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(IdentifyUser)
                && !string.IsNullOrWhiteSpace(LoginUser)
                && !string.IsNullOrWhiteSpace(IPAddress);
        }

        public override string GetSettingKey()
        {
            return "LogLogin_Insert";
        }
    }
}