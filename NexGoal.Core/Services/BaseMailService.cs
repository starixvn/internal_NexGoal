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
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MimeKit;
    using Newtonsoft.Json;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Models;

    public class BaseMailService : IBaseMailService
    {
        private readonly IConfiguration configuration;
        private readonly IOptionsSnapshot<MailConfigs> mailConfig;
        private readonly Fanex.Logging.ILogger fanexlogger;
        private readonly ILogger logger;

        public BaseMailService(
            IConfiguration configuration,
            IOptionsSnapshot<MailConfigs> mailConfig,
            Fanex.Logging.ILogger fanexlogger,
            ILogger<BaseMailService> logger)
        {
            this.configuration = configuration;
            this.mailConfig = mailConfig;
            this.fanexlogger = fanexlogger;
            this.logger = logger;
        }

        public async Task SendMailAsync(MailModel email)
        {
            if (email == null || !IsValidEmail(email.ReceiverEmail) || !mailConfig.Value.Enable)
            {
                return;
            }
            var systemName = configuration.GetSection("CompanyName").Value.Equals("Luminet") ? "LumiGoal" : "NexGoal";
            UpdateCompanyName(email, systemName);

            using (var mailclient = new SmtpClient())
            {
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(systemName, mailConfig.Value.Username));
                    message.To.Add(new MailboxAddress(email.ReceiverEmail));
                    message.Subject = email.Subject;
                    message.Priority = mailConfig.Value.MailPriority;

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
                    mailclient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    mailclient.CheckCertificateRevocation = false;
                    await mailclient.ConnectAsync(mailConfig.Value.SmtpServer, mailConfig.Value.SmtpPort, mailConfig.Value.EnableSsl);

                    if (mailConfig.Value.EnableSsl)
                    {
                        await mailclient.AuthenticateAsync(mailConfig.Value.Username, mailConfig.Value.Password);
                    }

                    await mailclient.SendAsync(message);

                    WriteLogSendEmail(email);
                }
                catch (AuthenticationException ex)
                {
                    await WriteLogErrorAsync(ex, email);
                }
                catch (SmtpCommandException ex)
                {
                    await WriteLogErrorAsync(ex, email);
                }
                catch (SmtpProtocolException ex)
                {
                    await WriteLogErrorAsync(ex, email);
                }
                catch (Exception ex)
                {
                    await WriteLogErrorAsync(ex, email);
                }

                await mailclient.DisconnectAsync(true);
            }
        }

        private void UpdateCompanyName(MailModel email, string systemName)
        {
            string body = email.Body.Replace("**SystemName**", systemName);
            email.Body = body;
        }

        public string ReadTextFile(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        public string FormatDate(DateTime date)
        {
            return date.ToString(Constants.DateFormat);
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

        private async Task WriteLogErrorAsync(Exception ex, MailModel email)
        {
            await fanexlogger.ErrorAsync(string.Format("{0}{1}{2}", ex.Message, Environment.NewLine, JsonConvert.SerializeObject(email)), ex);
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