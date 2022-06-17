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
        private readonly IChatbotService chatbotService;

        public PAFocusService(
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

        public async Task<CommandResult> GenerateAsync()
        {
            IEnumerable<int> periodIds = await GetPeriodStartedAsync();
            var criteria = new GenerateCriteria
            {
                PeriodIds = string.Join(',', periodIds)
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task SendMailAsync()
        {
            Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>> tuple = await GetSettingToSendNotificationAsync(ReceiveNotificationType.Email);

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
                                mailService.FormatDate(period.FocusEndDate),
                                $"{configuration.Value.ApplicationDomain}/pafocus");
                            await SendEmailAsync(subjectStartMail, body, focusSetting.Email);
                            break;

                        case MailMode.Remind:
                            body = string.Format(
                                mailService.ReadTextFile(remindMailPath),
                                focusSetting.Nickname,
                                period.Name,
                                mailService.FormatDate(period.FocusEndDate),
                                $"{configuration.Value.ApplicationDomain}/pafocus");
                            await SendEmailAsync(subjectRemindMail, body, focusSetting.Email);
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
            Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>> tuple = await GetSettingToSendNotificationAsync(ReceiveNotificationType.Notification);

            foreach (PeriodModel period in tuple.Item1)
            {
                IEnumerable<ProcessSettingModel> settings = tuple.Item2.Where(x => x.PeriodId == period.PeriodId);
                notifications.AddRange(settings.Select(x => new NotificationModel
                {
                    EmployeeId = x.EvaluatorId,
                    Content = x.NotificationMode == MailMode.Start
                    ? string.Format(Message.NotificationTemplate_PAFocus_Start, period.Name, mailService.FormatDate(period.FocusEndDate))
                    : string.Format(Message.NotificationTemplate_PAFocus_Remind, period.Name, mailService.FormatDate(period.FocusEndDate)),
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

                await baseService.ExecuteNonQueryAsync(criteria);
            }
        }

        public async Task SendSkypeMessage()
        {
            Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>> tuple = await GetSettingToSendNotificationAsync(ReceiveNotificationType.SkypeMessage);
            var message = string.Empty;

            foreach (PeriodModel period in tuple.Item1)
            {
                IEnumerable<ProcessSettingModel> settings = tuple.Item2.Where(x => x.PeriodId == period.PeriodId);

                foreach (ProcessSettingModel focusSetting in settings)
                {
                    switch (focusSetting.NotificationMode)
                    {
                        case MailMode.Start:
                            message = string.Format(Message.SkypeMessageTemplate_PAFocus_StartToEvaluator,
                                focusSetting.Nickname,
                                period.Name,
                                mailService.FormatDate(period.FocusEndDate),
                                configuration.Value.ApplicationDomain);
                            await chatbotService.SendMessageAsync(focusSetting.ConversationId, message);
                            break;

                        case MailMode.Remind:
                            message = string.Format(Message.SkypeMessageTemplate_PAFocus_RemindToEvaluator,
                                focusSetting.Nickname,
                                period.Name,
                                mailService.FormatDate(period.FocusEndDate),
                                configuration.Value.ApplicationDomain);
                            await chatbotService.SendMessageAsync(focusSetting.ConversationId, message);
                            break;

                        default:
                            continue;
                    }
                }
            }
        }

        private async Task<IEnumerable<int>> GetPeriodStartedAsync()
        {
            var criteria = new GetPeriodStartedCriteria();

            return await baseService.GetDataAsync<int>(criteria);
        }

        private async Task<Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>>> GetSettingToSendNotificationAsync(ReceiveNotificationType receiveType)
        {
            var criteria = new GetSettingToSendNotificationCriteria
            {
                ReceiveType = (int)receiveType
            };

            return await baseService.FetchMultipleAsync<PeriodModel, ProcessSettingModel>(criteria);
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