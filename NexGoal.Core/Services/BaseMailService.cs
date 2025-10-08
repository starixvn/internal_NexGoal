namespace NexGoal.Core.Services;

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
using Constants;
using Microsoft.Extensions.Configuration;
using Models;
using Newtonsoft.Json;

public class BaseMailService(
    IConfiguration configuration,
    MailConfigs mailConfig,
    Fanex.Logging.ILogger logger) : IBaseMailService
{
    public async Task SendMailAsync(MailModel email)
    {
        if (email == null || !IsValidEmail(email.ReceiverEmail) || !mailConfig.Enable)
        {
            return;
        }
        var appName = configuration.GetSection("AppName").Value;
        UpdateAppName(email, appName);

        try
        {
            var client = new SmtpClient(mailConfig.SmtpServer, mailConfig.SmtpPort);
            var msg = new MailMessage
            {
                From = new MailAddress(mailConfig.Username, appName)
            };
            msg.To.Add(email.ReceiverEmail);

            var mailCCs = email.Cc.Where(IsValidEmail).ToList();
            foreach (var mailcc in mailCCs)
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

            await WriteLogSendEmailAsync(email);
        }
        catch (Exception ex)
        {
            await WriteLogErrorAsync(ex, email);
        }
    }

    private static void UpdateAppName(MailModel email, string appName)
    {
        var body = email.Body.Replace("**SystemName**", appName);
        email.Body = body;
    }

    public string ReadTextFile(string filePath)
    {
        using var sr = new StreamReader(filePath);
        return sr.ReadToEnd();
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
        await logger.ErrorAsync($"{ex.Message}{Environment.NewLine}{JsonConvert.SerializeObject(email)}", ex);
    }

    private async Task WriteLogSendEmailAsync(MailModel email)
    {
        var message = string.Format(
            CultureInfo.InvariantCulture,
            "MailTo: {0}\n\rCc: {1}\n\rSubject: {2}",
            email.ReceiverEmail,
            string.Join(",", email.Cc.Where(IsValidEmail).Select(x => x)),
            email.Subject);

        await logger.InfoAsync(message);
    }

    private static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        => sslPolicyErrors == SslPolicyErrors.None;
}