namespace NexGoal.AutoService.Services
{
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Options;
    using Models;

    public class BaseMailService : IBaseMailService
    {
        private readonly IOptionsSnapshot<MailConfigs> configuration;

        public BaseMailService(IOptionsSnapshot<MailConfigs> configuration)
        {
            this.configuration = configuration;
        }

        public void SendEmail(MailModel email)
        {
            if (email == null || !IsValidEmail(email.ReceiverEmail))
            {
                return;
            }

            using (var message = new MailMessage())
            {
                message.From = new MailAddress(configuration.Value.Username);
                message.To.Add(new MailAddress(email.ReceiverEmail));
                message.Subject = email.Subject;
                message.Body = email.Body;
                message.IsBodyHtml = true;
                message.Priority = configuration.Value.MailPriority;

                foreach (Attachment attachment in email.Attachments)
                {
                    message.Attachments.Add(attachment);
                }

                using (var mailclient = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = configuration.Value.Username,
                        Password = configuration.Value.Password
                    };
                    mailclient.UseDefaultCredentials = true;
                    mailclient.Credentials = credential;
                    mailclient.Host = configuration.Value.SmtpServer;
                    mailclient.Port = configuration.Value.SmtpPort;
                    mailclient.EnableSsl = configuration.Value.EnableSsl;

                    mailclient.Send(message);
                }
            }
        }

        public string ReadTextFile(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regex.IsMatch(email);
        }
    }
}