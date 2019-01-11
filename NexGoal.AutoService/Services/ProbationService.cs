namespace NexGoal.AutoService.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Criterias;
    using NexGoal.AutoService.Criterias.PA;
    using NexGoal.AutoService.Models.Notification;
    using NexGoal.AutoService.Models.PA;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Resources;

    public class ProbationService : IProbationService
    {
        private const string RemindMailTemplateToMember = "EmailTemplates\\Probation\\RemindToMember.cshtml";
        private const string RemindMailTemplateToEvaluator = "EmailTemplates\\Probation\\RemindToEvaluator.cshtml";

        private readonly IOptionsSnapshot<AppSettings> configuration;
        private readonly IBaseService baseService;
        private readonly IBaseMailService mailService;

        public ProbationService(
            IOptionsSnapshot<AppSettings> configuration,
            IBaseService baseService,
            IBaseMailService mailService)
        {
            this.configuration = configuration;
            this.baseService = baseService;
            this.mailService = mailService;
        }

        public void SendMail()
        {
            IEnumerable<ProcessSettingModel> settings = GetSettingToSendNotification(ReceiveNotificationType.Email);

            var path = Directory.GetCurrentDirectory();
            var remindToMemberPath = Path.Combine(path, RemindMailTemplateToMember);
            var remindToEvaluatorPath = Path.Combine(path, RemindMailTemplateToEvaluator);
            var subjectRemindMail = Message.EmailSubject_Probation_RemindToMember;
            var body = string.Empty;

            foreach (ProcessSettingModel setting in settings)
            {
                switch (setting.ProcessId)
                {
                    case PerformanceProcess.Self:
                        body = string.Format(
                            mailService.ReadTextFile(remindToMemberPath),
                            setting.Nickname,
                            mailService.FormatStartDate(setting.EndDate),
                            configuration.Value.ApplicationDomain);
                        SendEmail(subjectRemindMail, body, setting.Email);
                        break;

                    case PerformanceProcess.Evaluator:
                        body = string.Format(
                            mailService.ReadTextFile(remindToEvaluatorPath),
                            setting.Nickname,
                            mailService.FormatStartDate(setting.EndDate),
                            configuration.Value.ApplicationDomain);
                        SendEmail(subjectRemindMail, body, setting.Email);
                        break;

                    default:
                        continue;
                }
            }
        }

        public void SendNotification()
        {
            var notifications = new List<NotificationModel>();
            IEnumerable<ProcessSettingModel> settings = GetSettingToSendNotification(ReceiveNotificationType.Notification);

            notifications.AddRange(settings.Select(x => new NotificationModel
            {
                EmployeeId = x.EvaluatorId,
                Content = x.ProcessId == PerformanceProcess.Self
                    ? string.Format(Message.NotificationTemplate_Probation_RemindToMember, mailService.FormatStartDate(x.EndDate))
                    : string.Format(Message.NotificationTemplate_Probation_RemindToEvaluator, mailService.FormatStartDate(x.EndDate)),
                StatusId = (int)NotificationStatus.Unread
            }));

            if (notifications.Any())
            {
                var criteria = new CreateNotificationCriteria
                {
                    Notifications = notifications.ToXML().ToString()
                };

                baseService.ExecuteNonQueryAsync(criteria).Wait();
            }
        }

        private IEnumerable<ProcessSettingModel> GetSettingToSendNotification(ReceiveNotificationType receiveType)
        {
            var criteria = new GetSettingToSendNotificationCriteria
            {
                ReceiveType = (int)receiveType
            };

            return baseService.GetDataAsync<ProcessSettingModel>(criteria).Result;
        }

        private void SendEmail(string subject, string body, string receiverEmail)
        {
            var email = new MailModel
            {
                Subject = subject,
                Body = body,
                ReceiverEmail = receiverEmail
            };

            mailService.SendMailAsync(email).Wait();
        }
    }
}