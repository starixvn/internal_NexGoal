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
        private const string StartMailTemplateToHR = "EmailTemplates\\Probation\\StartToHR.cshtml";
        private const string StartMailTemplateToMember = "EmailTemplates\\Probation\\StartToMember.cshtml";
        private const string StartMailTemplateToEvaluator = "EmailTemplates\\Probation\\StartToEvaluator.cshtml";
        private const string RemindMailTemplateToEvaluator = "EmailTemplates\\Probation\\RemindToEvaluator.cshtml";

        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IOptionsSnapshot<AppSettings> configuration;
        private readonly IBaseService baseService;
        private readonly IBaseMailService mailService;
        private readonly IChatbotService chatbotService;
        private readonly string memberPage;
        private readonly string probationSettingPage;

        public ProbationService(
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
            memberPage = $"{configuration.Value.ApplicationDomain}/member?activeTab=4";
            probationSettingPage = $"{configuration.Value.ApplicationDomain}/Probation/Setting";
        }

        public void SendMail()
        {
            IEnumerable<ProbationProcessSettingModel> settings = GetSettingToSendNotification(ReceiveNotificationType.Email);

            var startToHRPath = Path.Combine(hostingEnvironment.WebRootPath, StartMailTemplateToHR);
            var startToMemberPath = Path.Combine(hostingEnvironment.WebRootPath, StartMailTemplateToMember);
            var startToEvaluatorPath = Path.Combine(hostingEnvironment.WebRootPath, StartMailTemplateToEvaluator);
            var remindToEvaluatorPath = Path.Combine(hostingEnvironment.WebRootPath, RemindMailTemplateToEvaluator);
            var body = string.Empty;

            foreach (ProbationProcessSettingModel setting in settings)
            {
                switch (setting.ProcessId)
                {
                    case PerformanceProcess.HumanResource:
                        body = string.Format(
                            mailService.ReadTextFile(startToHRPath),
                            setting.ProbationerNickname,
                            probationSettingPage);
                        SendEmail(Message.EmailSubject_Probation_StartToHR, body, setting.Email);
                        break;

                    case PerformanceProcess.Self:
                        body = string.Format(
                            mailService.ReadTextFile(startToMemberPath),
                            setting.Nickname,
                            configuration.Value.ApplicationDomain,
                            mailService.FormatDate(setting.EndDate));
                        SendEmail(Message.EmailSubject_Probation_StartToEvaluator, body, setting.Email);
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
                        SendEmail(
                            setting.NotificationMode == MailMode.Start ? Message.EmailSubject_Probation_StartToEvaluator : Message.EmailSubject_Probation_RemindToEvaluator,
                            body, setting.Email);
                        break;

                    case PerformanceProcess.Reviewer:
                        body = string.Format(mailService.ReadTextFile(remindToEvaluatorPath),
                                  setting.Nickname,
                                  setting.ProbationerNickname,
                                  mailService.FormatDate(setting.EndDate),
                                  memberPage);
                        SendEmail(Message.EmailSubject_Probation_RemindToEvaluator, body, setting.Email);
                        break;

                    default:
                        continue;
                }
            }
        }

        public void SendNotification()
        {
            var notifications = new List<NotificationModel>();
            IEnumerable<ProbationProcessSettingModel> settings = GetSettingToSendNotification(ReceiveNotificationType.Notification);

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

                baseService.ExecuteNonQueryAsync(criteria).Wait();
            }
        }

        public async Task SendSkypeMessage()
        {
            IEnumerable<ProbationProcessSettingModel> settings = GetSettingToSendNotification(ReceiveNotificationType.SkypeMessage);
            var message = string.Empty;
            foreach (ProbationProcessSettingModel setting in settings)
            {
                switch (setting.ProcessId)
                {
                    case PerformanceProcess.HumanResource:
                        message = string.Format(Message.SkypeMessageTemplate_Probation_StartToHR,
                            setting.ProbationerNickname,
                            probationSettingPage);
                        await chatbotService.SendMessageAsync(setting.ConversationId, message).ConfigureAwait(false);
                        break;

                    case PerformanceProcess.Self:
                        message = string.Format(Message.SkypeMessageTemplate_Probation_StartToMember,
                            setting.Nickname,
                            configuration.Value.ApplicationDomain,
                            mailService.FormatDate(setting.EndDate));
                        await chatbotService.SendMessageAsync(setting.ConversationId, message).ConfigureAwait(false);
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

                        await chatbotService.SendMessageAsync(setting.ConversationId, message).ConfigureAwait(false);
                        break;

                    case PerformanceProcess.Reviewer:
                        message = string.Format(Message.SkypeMessageTemplate_Probation_StartToEvaluator,
                               setting.Nickname,
                               setting.ProbationerNickname,
                               memberPage,
                               mailService.FormatDate(setting.EndDate));
                        await chatbotService.SendMessageAsync(setting.ConversationId, message).ConfigureAwait(false);
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

        private IEnumerable<ProbationProcessSettingModel> GetSettingToSendNotification(ReceiveNotificationType receiveType)
        {
            var criteria = new GetSettingToSendNotificationCriteria
            {
                ReceiveType = (int)receiveType
            };

            var result = baseService.GetDataAsync<ProbationProcessSettingModel>(criteria).Result;

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

        private static int GetActionType(PerformanceProcess processId)
        {
            var actionTypeId = 0;

            switch (processId)
            {
                case PerformanceProcess.HumanResource:
                    actionTypeId = (int)NotificationActionType.ProbationSettingPage;
                    break;

                case PerformanceProcess.Evaluator:
                case PerformanceProcess.Reviewer:
                    actionTypeId = (int)NotificationActionType.MemberPageWithActiveProbationTab;
                    break;

                default:
                    actionTypeId = 0;
                    break;
            }

            return actionTypeId;
        }
    }
}