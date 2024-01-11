namespace NexGoal.AutoService.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.AutoService.Criterias;
    using NexGoal.AutoService.Criterias.Nomination;
    using NexGoal.AutoService.Models.Nomination;
    using NexGoal.AutoService.Models.Notification;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Resources;

    public class NominationService : INominationService
    {
        private const string StartMailTemplateToNominator = "EmailTemplates/Nomination/StartToNominator.txt";
        private const string RemindMailTemplateToNominator = "EmailTemplates/Nomination/RemindToNominator.txt";

        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly AppSettings appSettings;
        private readonly IBaseService baseService;
        private readonly IBaseMailService mailService;
        private readonly IChatbotService chatbotService;

        public NominationService(
            IWebHostEnvironment hostingEnvironment,
            AppSettings appSettings,
            IBaseService baseService,
            IBaseMailService mailService,
            IChatbotService chatbotService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.appSettings = appSettings;
            this.baseService = baseService;
            this.mailService = mailService;
            this.chatbotService = chatbotService;
        }

        public async Task SendMailAsync()
        {
            var (periods, processSettings) = await GetSettingToSendNotificationAsync(ReceiveNotificationType.Email);

            var startMailPath = Path.Combine(hostingEnvironment.ContentRootPath, StartMailTemplateToNominator);
            var remindMailPath = Path.Combine(hostingEnvironment.ContentRootPath, RemindMailTemplateToNominator);
            var body = string.Empty;

            foreach (var period in periods)
            {
                var settings = processSettings.Where(x => x.NominationPeriodId == period.NominationPeriodId).ToList();
                var subjectStartMail = string.Format(Message.EmailSubject_Nomination_Start, period.Title);
                var subjectRemindMail = string.Format(Message.EmailSubject_Nomination_Remind, period.Title);
                foreach (var setting in settings)
                {
                    switch (setting.NotificationMode)
                    {
                        case MailMode.Start:
                            body = string.Format(
                                mailService.ReadTextFile(startMailPath),
                                setting.Nickname,
                                period.Title,
                                mailService.FormatDate(period.EndDateForNominator),
                                $"{appSettings.ApplicationDomain}/member?activeTab=2");
                            await SendEmailAsync(subjectStartMail, body, setting.Email);
                            break;

                        case MailMode.Remind:
                            body = string.Format(
                                mailService.ReadTextFile(remindMailPath),
                                setting.Nickname,
                                period.Title,
                                mailService.FormatDate(period.EndDateForNominator),
                                $"{appSettings.ApplicationDomain}/member?activeTab=2");
                            await SendEmailAsync(subjectRemindMail, body, setting.Email);
                            break;

                        default:
                            continue;
                    }
                }
            }
        }

        public async Task SendNotificationAsync()
        {
            var notifications = new List<NotificationModel>();
            var (periods, processSettings) = await GetSettingToSendNotificationAsync(ReceiveNotificationType.Notification);

            foreach (var period in periods)
            {
                var settings = processSettings.Where(x => x.NominationPeriodId == period.NominationPeriodId);
                notifications.AddRange(settings.Select(x => new NotificationModel
                {
                    EmployeeId = x.NominatorId,
                    Content = x.NotificationMode == MailMode.Start
                        ? string.Format(Message.NotificationTemplate_Nomination_Start, period.Title, mailService.FormatDate(period.EndDateForNominator))
                        : string.Format(Message.NotificationTemplate_Nomination_Remind, period.Title, mailService.FormatDate(period.EndDateForNominator)),
                    StatusId = (byte)NotificationStatus.Unread,
                    ActionTypeId = (byte)NotificationActionType.MemberPageWithActiveNominationTab
                }));
            }

            if (notifications.Any())
            {
                var criteria = new CreateNotificationCriteria
                {
                    Notifications = notifications.ToXML().ToString()
                };

                await baseService.ExecuteNonQueryAsync(criteria);
            }
        }

        public async Task SendSkypeMessageAsync()
        {
            var (periods, processSettings) = await GetSettingToSendNotificationAsync(ReceiveNotificationType.SkypeMessage);

            foreach (var period in periods)
            {
                var settings = processSettings.Where(x => x.NominationPeriodId == period.NominationPeriodId).ToList();
                var message = string.Empty;
                foreach (var setting in settings)
                {
                    switch (setting.NotificationMode)
                    {
                        case MailMode.Start:
                            message = string.Format(Message.SkypeMessageTemplate_Nomination_StartForNominator,
                                setting.Nickname,
                                period.Title,
                                mailService.FormatDate(period.EndDateForNominator),
                                appSettings.ApplicationDomain);
                            await chatbotService.SendMessageAsync(setting.ConversationId, message);
                            break;

                        case MailMode.Remind:
                            message = string.Format(Message.SkypeMessageTemplate_Nomination_RemindToNominator,
                                setting.Nickname,
                                period.Title,
                                mailService.FormatDate(period.EndDateForNominator),
                                appSettings.ApplicationDomain);
                            await chatbotService.SendMessageAsync(setting.ConversationId, message);
                            break;

                        default:
                            continue;
                    }
                }
            }
        }

        public async Task<CommandResult> EditWhenFinishedAsync()
        {
            var criteria = new EditWhenFinishedCriteria();

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        private async Task<ValueTuple<IEnumerable<NominationPeriodModel>, IEnumerable<ProcessSettingModel>>> GetSettingToSendNotificationAsync(ReceiveNotificationType receiveType)
        {
            var criteria = new GetSettingToSendNotificationCriteria
            {
                ReceiveType = (int)receiveType
            };

            return await baseService.FetchMultipleAsync<NominationPeriodModel, ProcessSettingModel>(criteria);
        }

        private async Task SendEmailAsync(string subject, string body, string receiverEmail)
        {
            var email = new MailModel
            {
                Subject = subject,
                Body = body,
                ReceiverEmail = receiverEmail
            };

            await mailService.SendMailAsync(email);
        }
    }
}