namespace NexGoal.Core.Services
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Models;

    public class BaseMailService : IBaseMailService
    {
        private readonly IConfiguration configuration;
        private readonly MailConfigs mailConfig;
        private readonly Fanex.Logging.ILogger fanexlogger;
        private readonly ILogger logger;

        public BaseMailService(
            IConfiguration configuration,
            MailConfigs mailConfig,
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
            if (email == null || !IsValidEmail(email.ReceiverEmail) || !mailConfig.Enable)
            {
                return;
            }
            var systemName = configuration.GetSection("CompanyName").Value.Equals(Constants.LumiCompanyName) ? "LumiGoal" : "NexGoal";
            UpdateCompanyName(email, systemName);

            try
            {
                var client = new SmtpClient(mailConfig.SmtpServer, mailConfig.SmtpPort);
                var msg = new MailMessage
                {
                    From = new MailAddress(mailConfig.Username, systemName)
                };
                msg.To.Add(email.ReceiverEmail);

                var mailccs = email.Cc.Where(x => IsValidEmail(x)).ToList();
                foreach(var mailcc in mailccs)
                {
                    msg.CC.Add(mailcc);
                }
                msg.Subject = email.Subject;
                msg.SubjectEncoding = Encoding.UTF8;
                msg.IsBodyHtml = true;
                msg.Body = email.Body;
                msg.BodyEncoding = Encoding.UTF8;
                msg.Priority = MailPriority.Normal;

                ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;

                client.SendAsync(msg, null);

                WriteLogSendEmail(email);
            }
            catch (Exception ex)
            {
                await WriteLogErrorAsync(ex, email);
            }
        }

        private static void UpdateCompanyName(MailModel email, string systemName)
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

        private static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            => sslPolicyErrors == SslPolicyErrors.None;
    }
}