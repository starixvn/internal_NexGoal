namespace NexGoal.AutoService.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Options;
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
        private const string StartMailTemplateToNominator = "EmailTemplates\\Nomination\\StartToNominator.cshtml";
        private const string RemindMailTemplateToNominator = "EmailTemplates\\Nomination\\RemindToNominator.cshtml";

        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IOptionsSnapshot<AppSettings> configuration;
        private readonly IBaseService baseService;
        private readonly IBaseMailService mailService;
        private readonly IChatbotService chatbotService;

        public NominationService(
            IHostingEnvironment hostingEnvironment,
            IOptionsSnapshot<AppSettings> configuration,
            IBaseService baseService,
            IBaseMailService mailService,
            IChatbotService chatbotService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.configuration = configuration;
            this.baseService = baseService;
            this.mailService = mailService;
            this.chatbotService = chatbotService;
        }

        public async Task SendMailAsync()
        {
            Tuple<IEnumerable<NominationPeriodModel>, IEnumerable<ProcessSettingModel>> tuple = await GetSettingToSendNotificationAsync(ReceiveNotificationType.Email);

            var startMailPath = Path.Combine(hostingEnvironment.WebRootPath, StartMailTemplateToNominator);
            var remindMailPath = Path.Combine(hostingEnvironment.WebRootPath, RemindMailTemplateToNominator);
            var body = string.Empty;

            foreach (NominationPeriodModel period in tuple.Item1)
            {
                IEnumerable<ProcessSettingModel> settings = tuple.Item2.Where(x => x.NominationPeriodId == period.NominationPeriodId);
                var subjectStartMail = string.Format(Message.EmailSubject_Nomination_Start, period.Title);
                var subjectRemindMail = string.Format(Message.EmailSubject_Nomination_Remind, period.Title);
                foreach (ProcessSettingModel setting in settings)
                {
                    switch (setting.NotificationMode)
                    {
                        case MailMode.Start:
                            body = string.Format(
                                mailService.ReadTextFile(startMailPath),
                                setting.Nickname,
                                period.Title,
                                mailService.FormatDate(period.EndDateForNominator),
                                $"{configuration.Value.ApplicationDomain}/member?activeTab=2");
                            await SendEmailAsync(subjectStartMail, body, setting.Email);
                            break;

                        case MailMode.Remind:
                            body = string.Format(
                                mailService.ReadTextFile(remindMailPath),
                                setting.Nickname,
                                period.Title,
                                mailService.FormatDate(period.EndDateForNominator),
                                $"{configuration.Value.ApplicationDomain}/member?activeTab=2");
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
            Tuple<IEnumerable<NominationPeriodModel>, IEnumerable<ProcessSettingModel>> tuple = await GetSettingToSendNotificationAsync(ReceiveNotificationType.Notification);

            foreach (NominationPeriodModel period in tuple.Item1)
            {
                IEnumerable<ProcessSettingModel> settings = tuple.Item2.Where(x => x.NominationPeriodId == period.NominationPeriodId);
                notifications.AddRange(settings.Select(x => new NotificationModel
                {
                    EmployeeId = x.NominatorId,
                    Content = x.NotificationMode == MailMode.Start
                    ? string.Format(Message.NotificationTemplate_Nomination_Start, period.Title, mailService.FormatDate(period.EndDateForNominator))
                    : string.Format(Message.NotificationTemplate_Nomination_Remind, period.Title, mailService.FormatDate(period.EndDateForNominator)),
                    StatusId = (int)NotificationStatus.Unread,
                    ActionTypeId = (int)NotificationActionType.MemberPageWithActiveNominationTab
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
            Tuple<IEnumerable<NominationPeriodModel>, IEnumerable<ProcessSettingModel>> tuple = await GetSettingToSendNotificationAsync(ReceiveNotificationType.SkypeMessage);

            foreach (NominationPeriodModel period in tuple.Item1)
            {
                IEnumerable<ProcessSettingModel> settings = tuple.Item2.Where(x => x.NominationPeriodId == period.NominationPeriodId);
                string message = string.Empty;
                foreach (ProcessSettingModel setting in settings)
                {
                    switch (setting.NotificationMode)
                    {
                        case MailMode.Start:
                            message = string.Format(Message.SkypeMessageTemplate_Nomination_StartForNominator,
                                setting.Nickname,
                                period.Title,
                                mailService.FormatDate(period.EndDateForNominator),
                                configuration.Value.ApplicationDomain);
                            await chatbotService.SendMessageAsync(setting.ConversationId, message).ConfigureAwait(false);
                            break;

                        case MailMode.Remind:
                            message = string.Format(Message.SkypeMessageTemplate_Nomination_RemindToNominator,
                                setting.Nickname,
                                period.Title,
                                mailService.FormatDate(period.EndDateForNominator),
                                configuration.Value.ApplicationDomain);
                            await chatbotService.SendMessageAsync(setting.ConversationId, message).ConfigureAwait(false);
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

        private async Task<Tuple<IEnumerable<NominationPeriodModel>, IEnumerable<ProcessSettingModel>>> GetSettingToSendNotificationAsync(ReceiveNotificationType receiveType)
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