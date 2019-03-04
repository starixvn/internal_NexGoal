namespace NexGoal.Criteria.Auth
{
    public class WriteLogLoginCriteria : CommandBaseCriteria
    {
        public string Username { get; set; }
        public string IP { get; set; }
        public string MachineName { get; set; }
        public string Platform { get; set; }

        public override string GetSettingKey() => "LogUserLogin_Insert";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Username);
    }
}