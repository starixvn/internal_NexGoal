namespace NexGoal.Core.Services
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Text.RegularExpressions;
    using Constants;
    using Fanex.MailDeliver.MailClient.API;
    using Fanex.MailDeliver.Model;

    public class BaseMailService : IBaseMailService
    {
        public void SendEmail(MailModel email)
        {
            if (email == null
                || !IsValidEmail(email.ReceiverEmail)
                || ConfigurationManager.AppSettings["AllowSendMail"].Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var mailItem = new MailRecord
            {
                MailTo = email.ReceiverEmail,
                MailCc = email.MailCc,
                Subject = email.Subject,
                Title = ConfigurationManager.AppSettings["Email.SenderName"],
                Body = email.Body
            };
            using (var client = new MailDeliver())
            {
                client.SendMail(mailItem);
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

        public string ReadTextFile(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        public string ChangeStartDate(DateTime startDate)
        {
            var result = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, 0, startDate.Kind);
            return result.ToString(Constants.FormatDateTimeEmail);
        }

        public string ChangeEndDate(DateTime endDate)
        {
            var result = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 0, 0, endDate.Kind);
            return result.ToString(Constants.FormatDateTimeEmail);
        }
    }
}