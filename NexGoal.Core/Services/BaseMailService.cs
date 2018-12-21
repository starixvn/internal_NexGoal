namespace NexGoal.Core.Services
{
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using MailKit.Net.Smtp;
    using Microsoft.Extensions.Options;
    using MimeKit;
    using NexGoal.Core.Models;

    public class BaseMailService : IBaseMailService
    {
        private readonly IOptionsSnapshot<MailConfigs> configuration;

        public BaseMailService(IOptionsSnapshot<MailConfigs> configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendMailAsync(MailModel email)
        {
            if (email == null || !IsValidEmail(email.ReceiverEmail))
            {
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(configuration.Value.Username));
            message.To.Add(new MailboxAddress(email.ReceiverEmail));
            message.Subject = email.Subject;
            message.Priority = configuration.Value.MailPriority;

            if (email.Cc.Any())
            {
                message.Cc.AddRange(email.Cc.Select(x => new MailboxAddress(x)));
            }

            var builder = new BodyBuilder
            {
                HtmlBody = email.Body
            };
            foreach (MimeEntity attachment in email.Attachments)
            {
                builder.Attachments.Add(attachment);
            }

            message.Body = builder.ToMessageBody();

            using (var mailclient = new SmtpClient())
            {
                await mailclient.ConnectAsync(configuration.Value.SmtpServer, configuration.Value.SmtpPort, configuration.Value.EnableSsl).ConfigureAwait(false);

                if (configuration.Value.EnableSsl)
                {
                    await mailclient.AuthenticateAsync(configuration.Value.Username, configuration.Value.Password).ConfigureAwait(false);
                }

                await mailclient.SendAsync(message).ConfigureAwait(false);

                await mailclient.DisconnectAsync(true).ConfigureAwait(false);
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