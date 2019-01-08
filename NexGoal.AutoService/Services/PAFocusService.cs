namespace NexGoal.AutoService.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Criterias;
    using NexGoal.AutoService.Criterias.PAFocus;
    using NexGoal.AutoService.Models.Notification;
    using NexGoal.AutoService.Models.PAFocus;
    using NexGoal.AutoService.Models.Period;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Resources;

    public class PAFocusService : IPAFocusService
    {
        private const string StartMailTemplateToEvaluator = "EmailTemplates\\PAFocus\\StartToEvaluator.cshtml";
        private const string RemindMailTemplateToEvaluator = "EmailTemplates\\PAFocus\\RemindToEvaluator.cshtml";

        private readonly IBaseService baseService;
        private readonly IBaseMailService mailService;
        private readonly IOptionsSnapshot<AppSettings> configuration;

        public PAFocusService(
            IOptionsSnapshot<AppSettings> configuration,
            IBaseService baseService,
            IBaseMailService mailService)
        {
            this.configuration = configuration;
            this.baseService = baseService;
            this.mailService = mailService;
        }

        public CommandResult Generate()
        {
            IEnumerable<int> periodIds = GetPeriodStarted();
            var criteria = new GenerateCriteria
            {
                PeriodIds = string.Join(',', periodIds)
            };

            return baseService.ExecuteNonQueryAsync(criteria).Result;
        }

        public void SendMail()
        {
            Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>> tuple = GetSettingToSendNotification(ReceiveNotificationType.Email);

            var path = Directory.GetCurrentDirectory();
            var startMailPath = Path.Combine(path, StartMailTemplateToEvaluator);
            var remindMailPath = Path.Combine(path, RemindMailTemplateToEvaluator);
            var body = string.Empty;

            foreach (PeriodModel period in tuple.Item1)
            {
                IEnumerable<ProcessSettingModel> settings = tuple.Item2.Where(x => x.PeriodId == period.PeriodId);
                var subjectStartMail = string.Format(Message.EmailSubject_PAFocus_Start, period.Name);
                var subjectRemindMail = string.Format(Message.EmailSubject_PAFocus_Remind, period.Name);

                foreach (ProcessSettingModel focusSetting in settings)
                {
                    switch (focusSetting.NotificationMode)
                    {
                        case MailMode.Start:
                            body = string.Format(
                                mailService.ReadTextFile(startMailPath),
                                focusSetting.Nickname,
                                period.Name,
                                configuration.Value.ApplicationDomain);
                            SendEmail(subjectStartMail, body, focusSetting.Email);
                            break;

                        case MailMode.Remind:
                            body = string.Format(
                                mailService.ReadTextFile(remindMailPath),
                                focusSetting.Nickname,
                                period.Name,
                                mailService.FormatStartDate(period.FocusEndDate),
                                configuration.Value.ApplicationDomain);
                            SendEmail(subjectRemindMail, body, focusSetting.Email);
                            break;

                        default:
                            continue;
                    }
                }
            }
        }

        public void CreateNotification()
        {
            var notifications = new List<NotificationModel>();
            Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>> tuple = GetSettingToSendNotification(ReceiveNotificationType.Notification);

            foreach (PeriodModel period in tuple.Item1)
            {
                IEnumerable<ProcessSettingModel> settings = tuple.Item2.Where(x => x.PeriodId == period.PeriodId);
                notifications.AddRange(settings.Select(x => new NotificationModel
                {
                    EmployeeId = x.EvaluatorId,
                    Content = x.NotificationMode == MailMode.Start
                    ? string.Format(Message.NotificationTemplate_PAFocus_Start, period.Name)
                    : string.Format(Message.NotificationTemplate_PAFocus_Remind, period.Name, mailService.FormatEndDate(period.FocusEndDate)),
                    StatusId = (int)NotificationStatus.Unread
                }));
            }

            if (notifications.Any())
            {
                var criteria = new CreateNotificationCriteria
                {
                    Notifications = notifications.ToXML().ToString()
                };

                baseService.ExecuteNonQueryAsync(criteria).Wait();
            }
        }

        private IEnumerable<int> GetPeriodStarted()
        {
            var criteria = new GetPeriodStartedCriteria();

            return baseService.GetDataAsync<int>(criteria).Result;
        }

        private Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>> GetSettingToSendNotification(ReceiveNotificationType receiveType)
        {
            var criteria = new GetSettingToSendNotificationCriteria
            {
                ReceiveType = (int)receiveType
            };

            return baseService.FetchMultipleAsync<PeriodModel, ProcessSettingModel>(criteria).Result;
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