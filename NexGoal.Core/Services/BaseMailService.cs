namespace NexGoal.Core.Services
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MimeKit;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Models;

    public class BaseMailService : IBaseMailService
    {
        private readonly IOptionsSnapshot<MailConfigs> configuration;
        private readonly Fanex.Logging.ILogger fanexlogger;
        private readonly ILogger logger;

        public BaseMailService(
            IOptionsSnapshot<MailConfigs> configuration,
            Fanex.Logging.ILogger fanexlogger,
            ILogger<BaseMailService> logger)
        {
            this.configuration = configuration;
            this.fanexlogger = fanexlogger;
            this.logger = logger;
        }

        public async Task SendMailAsync(MailModel email)
        {
            if (email == null || !IsValidEmail(email.ReceiverEmail) || !configuration.Value.Enable)
            {
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("NexGoal", configuration.Value.Username));
            message.To.Add(new MailboxAddress(email.ReceiverEmail));
            message.Subject = email.Subject;
            message.Priority = configuration.Value.MailPriority;

            if (email.Cc.Any(x => IsValidEmail(x)))
            {
                message.Cc.AddRange(email.Cc.Where(x => IsValidEmail(x)).Select(x => new MailboxAddress(x)));
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

                try
                {
                    await mailclient.SendAsync(message).ConfigureAwait(false);

                    WriteLogSendEmail(email);
                }
                catch (AuthenticationException ex)
                {
                    fanexlogger.Error(ex.Message, ex);
                }
                catch (SmtpCommandException ex)
                {
                    fanexlogger.Error(ex.Message, ex);
                }
                catch (SmtpProtocolException ex)
                {
                    fanexlogger.Error(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    fanexlogger.Error(ex.Message, ex);
                }

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

        public string FormatStartDate(DateTime startDate)
        {
            var result = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, 0, startDate.Kind);

            return result.ToString(Constants.DateTimeFormat);
        }

        public string FormatEndDate(DateTime endDate)
        {
            var result = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 0, 0, endDate.Kind);

            return result.ToString(Constants.DateTimeFormat);
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

        private void WriteLogSendEmail(MailModel email)
        {
            string message = string.Format(
                        CultureInfo.InvariantCulture,
                        "MailTo: {0}\n\rCc: {1}\n\rSubject: {2}",
                        email.ReceiverEmail,
                        string.Join(",", email.Cc.Where(x => IsValidEmail(x)).Select(x => x)),
                        email.Subject);

            logger.LogInformation(message);
        }
    }
}