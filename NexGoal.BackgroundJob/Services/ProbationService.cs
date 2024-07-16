namespace NexGoal.BackgroundJob.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Criterias;
    using Criterias.Probation;
    using Microsoft.AspNetCore.Hosting;
    using Models.Notification;
    using Models.PA;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Resources;

    public class ProbationService : IProbationService
    {
        private const string StartMailTemplateToHR = "EmailTemplates/Probation/StartToHR.txt";
        private const string StartMailTemplateToMember = "EmailTemplates/Probation/StartToMember.txt";
        private const string StartMailTemplateToEvaluator = "EmailTemplates/Probation/StartToEvaluator.txt";
        private const string RemindMailTemplateToEvaluator = "EmailTemplates/Probation/RemindToEvaluator.txt";

        private readonly IHostEnvironment hostingEnvironment;
        private readonly AppSettings appSettings;
        private readonly IBaseService baseService;
        private readonly IBaseMailService mailService;
        private readonly IChatbotService chatbotService;
        private readonly string memberPage;
        private readonly string probationSettingPage;

        public ProbationService(
            IHostEnvironment hostingEnvironment,
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
            memberPage = $"{appSettings.ApplicationDomain}/member?activeTab=4";
            probationSettingPage = $"{appSettings.ApplicationDomain}/Probation/Setting";
        }

        public async Task SendMailAsync()
        {
            IEnumerable<ProbationProcessSettingModel> settings = await GetSettingToSendNotificationAsync(ReceiveNotificationType.Email);

            var startToHRPath = Path.Combine(hostingEnvironment.ContentRootPath, StartMailTemplateToHR);
            var startToMemberPath = Path.Combine(hostingEnvironment.ContentRootPath, StartMailTemplateToMember);
            var startToEvaluatorPath = Path.Combine(hostingEnvironment.ContentRootPath, StartMailTemplateToEvaluator);
            var remindToEvaluatorPath = Path.Combine(hostingEnvironment.ContentRootPath, RemindMailTemplateToEvaluator);
            string body;

            foreach (ProbationProcessSettingModel setting in settings)
            {
                switch (setting.ProcessId)
                {
                    case PerformanceProcess.HumanResource:
                        body = string.Format(
                            mailService.ReadTextFile(startToHRPath),
                            setting.ProbationerNickname,
                            probationSettingPage);
                        await SendEmailAsync(Message.EmailSubject_Probation_StartToHR, body, setting.Email);
                        break;

                    case PerformanceProcess.Self:
                        body = string.Format(
                            mailService.ReadTextFile(startToMemberPath),
                            setting.Nickname,
                            appSettings.ApplicationDomain,
                            mailService.FormatDate(setting.EndDate));
                        await SendEmailAsync(Message.EmailSubject_Probation_StartToEvaluator, body, setting.Email);
                        break;

                    case PerformanceProcess.Evaluator:
                        body = setting.NotificationMode == MailMode.Start ?
                            string.Format(mailService.ReadTextFile(startToEvaluatorPath),
                                setting.Nickname,
                                setting.ProbationerNickname,
                                memberPage,
                                mailService.FormatDate(setting.EndDate))
                          : string.Format(mailService.ReadTextFile(remindToEvaluatorPath),
                                setting.Nickname,
                                setting.ProbationerNickname,
                                mailService.FormatDate(setting.EndDate),
                                memberPage);
                        await SendEmailAsync(
                            setting.NotificationMode == MailMode.Start ? Message.EmailSubject_Probation_StartToEvaluator : Message.EmailSubject_Probation_RemindToEvaluator,
                            body, setting.Email);
                        break;

                    case PerformanceProcess.Reviewer:
                        body = string.Format(mailService.ReadTextFile(remindToEvaluatorPath),
                                  setting.Nickname,
                                  setting.ProbationerNickname,
                                  mailService.FormatDate(setting.EndDate),
                                  memberPage);
                        await SendEmailAsync(Message.EmailSubject_Probation_RemindToEvaluator, body, setting.Email);
                        break;

                    default:
                        continue;
                }
            }
        }

        public async Task SendNotificationAsync()
        {
            var notifications = new List<NotificationModel>();
            IEnumerable<ProbationProcessSettingModel> settings = await GetSettingToSendNotificationAsync(ReceiveNotificationType.Notification);

            notifications.AddRange(settings.Select(x => new NotificationModel
            {
                EmployeeId = x.EvaluatorId,
                Content = GetNotificationContent(x),
                StatusId = (int)NotificationStatus.Unread,
                ActionTypeId = GetActionType(x.ProcessId)
            }));

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
            IEnumerable<ProbationProcessSettingModel> settings = await GetSettingToSendNotificationAsync(ReceiveNotificationType.SkypeMessage);
            var message = string.Empty;
            foreach (ProbationProcessSettingModel setting in settings)
            {
                switch (setting.ProcessId)
                {
                    case PerformanceProcess.HumanResource:
                        message = string.Format(Message.SkypeMessageTemplate_Probation_StartToHR,
                            setting.ProbationerNickname,
                            probationSettingPage);
                        await chatbotService.SendMessageAsync(setting.ConversationId, message);
                        break;

                    case PerformanceProcess.Self:
                        message = string.Format(Message.SkypeMessageTemplate_Probation_StartToMember,
                            setting.Nickname,
                            appSettings.ApplicationDomain,
                            mailService.FormatDate(setting.EndDate));
                        await chatbotService.SendMessageAsync(setting.ConversationId, message);
                        break;

                    case PerformanceProcess.Evaluator:
                        message = setting.NotificationMode == MailMode.Start ?
                             string.Format(Message.SkypeMessageTemplate_Probation_StartToEvaluator,
                                setting.Nickname,
                                setting.ProbationerNickname,
                                memberPage,
                                mailService.FormatDate(setting.EndDate))
                            : string.Format(Message.SkypeMessageTemplate_Probation_RemindEvaluator,
                                setting.Nickname,
                                setting.ProbationerNickname,
                                mailService.FormatDate(setting.EndDate),
                                memberPage);

                        await chatbotService.SendMessageAsync(setting.ConversationId, message);
                        break;

                    case PerformanceProcess.Reviewer:
                        message = string.Format(Message.SkypeMessageTemplate_Probation_StartToEvaluator,
                               setting.Nickname,
                               setting.ProbationerNickname,
                               memberPage,
                               mailService.FormatDate(setting.EndDate));
                        await chatbotService.SendMessageAsync(setting.ConversationId, message);
                        break;

                    default:
                        continue;
                }
            }
        }

        public async Task AddNewEmployeeToProbation()
        {
            var criteria = new AddNewEmployeeToProbationCriteria();

            await baseService.ExecuteNonQueryAsync(criteria);
        }

        private async Task<IEnumerable<ProbationProcessSettingModel>> GetSettingToSendNotificationAsync(ReceiveNotificationType receiveType)
        {
            var criteria = new GetSettingToSendNotificationCriteria
            {
                ReceiveType = (int)receiveType
            };

            var result = await baseService.GetDataAsync<ProbationProcessSettingModel>(criteria);

            return result.GroupBy(x => new
            {
                x.ProcessId,
                x.NotificationMode,
                x.EvaluatorId,
                x.Nickname,
                x.Email,
                x.StartDate,
                x.EndDate,
                x.ConversationId
            }, x => x.ProbationerNickname,
            (key, probationerNickname) => new ProbationProcessSettingModel
            {
                ProcessId = key.ProcessId,
                NotificationMode = key.NotificationMode,
                EvaluatorId = key.EvaluatorId,
                Nickname = key.Nickname,
                Email = key.Email,
                StartDate = key.StartDate,
                EndDate = key.EndDate,
                ConversationId = key.ConversationId,
                ProbationerNickname = string.Join(", ", probationerNickname)
            });
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

        private string GetNotificationContent(ProbationProcessSettingModel model)
        {
            var content = string.Empty;

            switch (model.ProcessId)
            {
                case PerformanceProcess.HumanResource:
                    content = string.Format(Message.NotificationTemplate_Probation_StartToHR, model.ProbationerNickname);
                    break;

                case PerformanceProcess.Self:
                    content = string.Format(Message.NotificationTemplate_Probation_StartToMember, mailService.FormatDate(model.EndDate));
                    break;

                case PerformanceProcess.Evaluator:
                    content = model.NotificationMode == MailMode.Start ?
                         string.Format(Message.NotificationTemplate_Probation_StartToEvaluator, model.ProbationerNickname, mailService.FormatDate(model.EndDate)) :
                         string.Format(Message.NotificationTemplate_Probation_RemindToEvaluator, model.ProbationerNickname, mailService.FormatDate(model.EndDate));
                    break;

                case PerformanceProcess.Reviewer:
                    content = string.Format(Message.NotificationTemplate_Probation_StartToEvaluator, model.ProbationerNickname, mailService.FormatDate(model.EndDate));
                    break;

                default:
                    content = string.Format(Message.NotificationTemplate_Probation_RemindToMember, mailService.FormatDate(model.EndDate));
                    break;
            }

            return content;
        }

        private static byte GetActionType(PerformanceProcess processId)
        {
            byte actionTypeId;
            switch (processId)
            {
                case PerformanceProcess.HumanResource:
                    actionTypeId = (byte)NotificationActionType.ProbationSettingPage;
                    break;

                case PerformanceProcess.Evaluator:
                case PerformanceProcess.Reviewer:
                    actionTypeId = (byte)NotificationActionType.MemberPageWithActiveProbationTab;
                    break;

                default:
                    actionTypeId = 0;
                    break;
            }

            return actionTypeId;
        }
    }
}