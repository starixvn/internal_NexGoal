namespace NexGoal.Core.Models
{
    using System.Net.Mail;

    public class MailConfigs
    {
        public bool Enable { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
        public MailPriority MailPriority { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}