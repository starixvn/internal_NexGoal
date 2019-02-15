namespace NexGoal.AutoService.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Criterias;
    using NexGoal.AutoService.Criterias.Probation;
    using NexGoal.AutoService.Models.Notification;
    using NexGoal.AutoService.Models.PA;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Resources;

    public class ProbationService : IProbationService
    {
        private const string RemindMailTemplateToHR = "EmailTemplates\\Probation\\RemindToHR.cshtml";
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
            var remindToHRPath = Path.Combine(path, RemindMailTemplateToHR);
            var remindToMemberPath = Path.Combine(path, RemindMailTemplateToMember);
            var remindToEvaluatorPath = Path.Combine(path, RemindMailTemplateToEvaluator);
            var subjectRemindMail = Message.EmailSubject_Probation_RemindToMember;
            var body = string.Empty;

            foreach (ProcessSettingModel setting in settings)
            {
                switch (setting.ProcessId)
                {
                    case PerformanceProcess.HumanResource:
                        body = string.Format(
                            mailService.ReadTextFile(remindToHRPath),
                            setting.Nickname,
                            mailService.FormatEndDate(setting.EndDate));
                        SendEmail(subjectRemindMail, body, setting.Email);
                        break;

                    case PerformanceProcess.Self:
                        body = string.Format(
                            mailService.ReadTextFile(remindToMemberPath),
                            setting.Nickname,
                            mailService.FormatEndDate(setting.EndDate),
                            configuration.Value.ApplicationDomain);
                        SendEmail(subjectRemindMail, body, setting.Email);
                        break;

                    case PerformanceProcess.Evaluator:
                    case PerformanceProcess.Reviewer:
                        body = string.Format(
                            mailService.ReadTextFile(remindToEvaluatorPath),
                            setting.Nickname,
                            mailService.FormatEndDate(setting.EndDate),
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
                Content = GetNotificationContent(x.ProcessId, x.EndDate),
                StatusId = (int)NotificationStatus.Unread,
                ActionTypeId = GetActionType(x.ProcessId)
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

        private string GetNotificationContent(PerformanceProcess processId, DateTime endDate)
        {
            var content = string.Empty;

            switch (processId)
            {
                case PerformanceProcess.HumanResource:
                    content = string.Format(Message.NotificationTemplate_Probation_RemindToHR, mailService.FormatEndDate(endDate));
                    break;

                case PerformanceProcess.Evaluator:
                case PerformanceProcess.Reviewer:
                    content = string.Format(Message.NotificationTemplate_Probation_RemindToEvaluator, mailService.FormatEndDate(endDate));
                    break;

                default:
                    content = string.Format(Message.NotificationTemplate_Probation_RemindToMember, mailService.FormatEndDate(endDate));
                    break;
            }

            return content;
        }

        private static int GetActionType(PerformanceProcess processId)
        {
            var actionTypeId = 0;

            switch (processId)
            {
                case PerformanceProcess.Self:
                    actionTypeId = (int)NotificationActionType.MyPageWithCreatePerformanceCheckin;
                    break;

                case PerformanceProcess.Evaluator:
                case PerformanceProcess.Reviewer:
                    actionTypeId = (int)NotificationActionType.MemberPageWithCreatePerformanceCheckin;
                    break;

                default:
                    actionTypeId = 0;
                    break;
            }

            return actionTypeId;
        }
    }
}