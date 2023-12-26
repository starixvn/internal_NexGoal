namespace NexGoal.Models.Auth
{
    public class LdapSettings
    {
        public string Domain { get; set; }
        public string Host { get; set; } = string.Empty;
        public short Port { get; set; }
        public string LoginSchema { get; set; }
    }
}