namespace NexGoal.AutoService.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.AspNetCore.Hosting;
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

        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IOptionsSnapshot<AppSettings> configuration;
        private readonly IBaseService baseService;
        private readonly IBaseMailService mailService;

        public PAFocusService(
            IHostingEnvironment hostingEnvironment,
            IOptionsSnapshot<AppSettings> configuration,
            IBaseService baseService,
            IBaseMailService mailService)
        {
            this.hostingEnvironment = hostingEnvironment;
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

            var startMailPath = Path.Combine(hostingEnvironment.WebRootPath, StartMailTemplateToEvaluator);
            var remindMailPath = Path.Combine(hostingEnvironment.WebRootPath, RemindMailTemplateToEvaluator);
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

        public void SendNotification()
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
                    StatusId = (int)NotificationStatus.Unread,
                    ActionTypeId = (int)NotificationActionType.PAFocus
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