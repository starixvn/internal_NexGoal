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
    using NexGoal.AutoService.Criterias.PA;
    using NexGoal.AutoService.Models;
    using NexGoal.AutoService.Models.Notification;
    using NexGoal.AutoService.Models.PA;
    using NexGoal.AutoService.Models.Period;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Resources;

    public class PAService : IPAService
    {
        private const string RemindStartEvaluationMailTemplateToSelf = "EmailTemplates\\PA\\RemindStartToSelf.cshtml";
        private const string RemindStartEvaluationMailTemplateToSupervisor = "EmailTemplates\\PA\\RemindStartToSupervisor.cshtml";
        private const string StartMailTemplateToReviewer = "EmailTemplates\\PA\\StartToReviewer.cshtml";
        private const string RemindMailTemplateToReviewer = "EmailTemplates\\PA\\RemindToReviewer.cshtml";
        private const string EndEvaluationMailTemplateToSelf = "EmailTemplates\\PA\\EndToSelf.cshtml";

        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IOptionsSnapshot<AppSettings> configuration;
        private readonly IBaseService baseService;
        private readonly IBaseMailService mailService;
        private readonly ICryptoService cryptoService;
        private readonly IChatbotService chatbotService;

        public PAService(
            IHostingEnvironment hostingEnvironment,
            IOptionsSnapshot<AppSettings> configuration,
            IBaseService baseService,
            IBaseMailService mailService,
            ICryptoService cryptoService,
            IChatbotService chatbotService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.configuration = configuration;
            this.baseService = baseService;
            this.mailService = mailService;
            this.cryptoService = cryptoService;
            this.chatbotService = chatbotService;
        }

        public async Task<CommandResult> EditGradeAsync()
        {
            var commandResult = new CommandResult();
            IEnumerable<EditPAGradeDto> records = await GetDataToEditGradeAsync();

            if (records != null && records.Any())
            {
                var criteria = new EditGradeCriteria { Value = records.ToXML().ToString() };

                commandResult = await baseService.ExecuteNonQueryAsync(criteria);
            }

            return commandResult;
        }

        public async Task<CommandResult> EditWhenEvaluationFinishedAsync()
        {
            var criteria = new EditWhenEvaluationFinishedCriteria();

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<CommandResult> EditStatusAsync()
        {
            var criteria = new EditStatusCriteria();

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task SendMailAsync()
        {
            Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>> tuple = await GetSettingToSendNotificationAsync(ReceiveNotificationType.Email);

            var remindStartEvaluationToSelfPath = Path.Combine(hostingEnvironment.WebRootPath, RemindStartEvaluationMailTemplateToSelf);
            var remindStartEvaluationToSupervisorPath = Path.Combine(hostingEnvironment.WebRootPath, RemindStartEvaluationMailTemplateToSupervisor);
            var startToReviewerPath = Path.Combine(hostingEnvironment.WebRootPath, StartMailTemplateToReviewer);
            var remindToReviewerPath = Path.Combine(hostingEnvironment.WebRootPath, RemindMailTemplateToReviewer);
            var endEvaluationMailPath = Path.Combine(hostingEnvironment.WebRootPath, EndEvaluationMailTemplateToSelf);

            foreach (PeriodModel period in tuple.Item1)
            {
                IEnumerable<ProcessSettingModel> settings = tuple.Item2.Where(x => x.PeriodId == period.PeriodId);
                var subjectRemindStartEvaluationMail = string.Format(Message.EmailSubject_PA_RemindStartEvaluation, period.Name);
                var subjectStartMail = string.Format(Message.EmailSubject_PA_StartToReviewer, period.Name);
                var subjectRemindMail = string.Format(Message.EmailSubject_PA_RemindToReviewer, period.Name);
                var subjectEndEvaluationMail = string.Format(Message.EmailSubject_PA_EndEvaluation, period.Name);
                var body = string.Empty;

                foreach (ProcessSettingModel setting in settings)
                {
                    switch (setting.NotificationMode)
                    {
                        case MailMode.RemindStartEvaluation:
                            body = string.Format(
                                mailService.ReadTextFile(
                                    setting.ProcessId == PerformanceProcess.Self
                                    ? remindStartEvaluationToSelfPath
                                    : remindStartEvaluationToSupervisorPath),
                                setting.Nickname,
                                mailService.FormatDate(period.StartDate));
                            await SendEmailAsync(subjectRemindStartEvaluationMail, body, setting);
                            break;

                        case MailMode.Start:
                            body = string.Format(
                                mailService.ReadTextFile(startToReviewerPath),
                                setting.Nickname,
                                period.Name,
                                mailService.FormatDate(setting.EndDate),
                                $"{ configuration.Value.ApplicationDomain}/member?activeTab=3");
                            await SendEmailAsync(subjectStartMail, body, setting);
                            break;

                        case MailMode.Remind:
                            body = string.Format(
                                mailService.ReadTextFile(remindToReviewerPath),
                                setting.Nickname,
                                period.Name,
                                mailService.FormatDate(setting.EndDate),
                                $"{ configuration.Value.ApplicationDomain}/member?activeTab=3");
                            await SendEmailAsync(subjectRemindMail, body, setting);
                            break;

                        case MailMode.EndEvaluation:
                            body = string.Format(
                                mailService.ReadTextFile(endEvaluationMailPath),
                                setting.Nickname,
                                period.Name,
                                configuration.Value.ApplicationDomain);
                            await SendEmailAsync(subjectEndEvaluationMail, body, setting);
                            break;

                        default:
                            continue;
                    }
                }
            }
        }

        public async Task SendSkypeMessage()
        {
            Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>> tuple = await GetSettingToSendNotificationAsync(ReceiveNotificationType.SkypeMessage);

            foreach (PeriodModel period in tuple.Item1)
            {
                IEnumerable<ProcessSettingModel> settings = tuple.Item2.Where(x => x.PeriodId == period.PeriodId);
                var message = string.Empty;

                foreach (ProcessSettingModel setting in settings)
                {
                    switch (setting.NotificationMode)
                    {
                        case MailMode.RemindStartEvaluation:
                            var template = setting.ProcessId == PerformanceProcess.Self ? Message.SkypeMessageTemplate_PA_RemindStartEvaluationToSelf
                                : Message.SkypeMessageTemplate_PA_RemindStartEvaluationToSupervisor;
                            message = string.Format(template, setting.Nickname, mailService.FormatDate(period.StartDate));

                            await chatbotService.SendMessageAsync(setting.ConversationId, message).ConfigureAwait(false);
                            break;

                        case MailMode.Start:
                            message = string.Format(Message.SkypeMessageTemplate_PA_StartForReviewer,
                                setting.Nickname,
                                period.Name,
                                mailService.FormatDate(setting.EndDate),
                                configuration.Value.ApplicationDomain);
                            await chatbotService.SendMessageAsync(setting.ConversationId, message).ConfigureAwait(false);

                            break;

                        case MailMode.Remind:
                            message = string.Format(Message.SkypeMessageTemplate_PA_RemindForReviewer,
                                setting.Nickname,
                                period.Name,
                                mailService.FormatDate(setting.EndDate),
                                configuration.Value.ApplicationDomain);
                            await chatbotService.SendMessageAsync(setting.ConversationId, message).ConfigureAwait(false);

                            break;

                        case MailMode.EndEvaluation:
                            message = string.Format(Message.SkypeMessageTemplate_PA_EndEvaluation,
                                setting.Nickname,
                                period.Name,
                                configuration.Value.ApplicationDomain);
                            await chatbotService.SendMessageAsync(setting.ConversationId, message).ConfigureAwait(false);

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
                foreach (ProcessSettingModel setting in settings)
                {
                    switch (setting.NotificationMode)
                    {
                        case MailMode.RemindStartEvaluation:
                            notifications.Add(new NotificationModel
                            {
                                EmployeeId = setting.EvaluatorId,
                                Content = string.Format(
                                    setting.ProcessId == PerformanceProcess.Self
                                        ? Message.NotificationTemplate_PA_RemindStartEvaluationToMember
                                        : Message.NotificationTemplate_PA_RemindStartEvaluationToSupervisor,
                                    mailService.FormatDate(period.StartDate)),
                                StatusId = (int)NotificationStatus.Unread,
                                ActionTypeId = (int)NotificationActionType.MyPageWithCreatePerformanceCheckin
                            });
                            break;

                        case MailMode.Start:
                            notifications.Add(new NotificationModel
                            {
                                EmployeeId = setting.EvaluatorId,
                                Content = string.Format(
                                    Message.NotificationTemplate_PA_StartToReviewer,
                                    period.Name,
                                    mailService.FormatDate(setting.EndDate)),
                                StatusId = (int)NotificationStatus.Unread,
                                ActionTypeId = (int)NotificationActionType.MemberPageWithActivePerformanceAppraisalTab
                            });
                            break;

                        case MailMode.Remind:
                            notifications.Add(new NotificationModel
                            {
                                EmployeeId = setting.EvaluatorId,
                                Content = string.Format(Message.NotificationTemplate_PA_RemindToReviewer,
                                    period.Name,
                                    mailService.FormatDate(setting.EndDate)),
                                StatusId = (int)NotificationStatus.Unread,
                                ActionTypeId = (int)NotificationActionType.MemberPageWithActivePerformanceAppraisalTab
                            });
                            break;

                        case MailMode.EndEvaluation:
                            notifications.Add(new NotificationModel
                            {
                                EmployeeId = setting.EvaluatorId,
                                Content = string.Format(Message.NotificationTemplate_PA_EndEvaluation, period.Name),
                                StatusId = (int)NotificationStatus.Unread,
                                ActionTypeId = (int)NotificationActionType.MyPageWithSpecificPerformanceCheckin,
                                RecordId = setting.PerformanceRecordId
                            });
                            break;

                        default:
                            continue;
                    }
                }
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

        private async Task<IEnumerable<EditPAGradeDto>> GetDataToEditGradeAsync()
        {
            var criteria = new GetDataToEditGradeCriteria();
            var records = await baseService.GetDataAsync<EditPAGradeDto>(criteria);
            var grades = await baseService.GetDataAsync<PerformanceGrade>(new GetPerformanceGradeCriteria());

            foreach (EditPAGradeDto record in records)
            {
                switch (record.StatusId)
                {
                    case (int)PARecordStatus.InProgress:
                        CalculatePAGrade(record, grades);
                        break;

                    case (int)PARecordStatus.Closed:
                        CalculateNullFinalPAGrade(record, grades);

                        break;

                    default:
                        continue;
                }
            }

            return records;
        }

        private void CalculatePAGrade(EditPAGradeDto record, IEnumerable<PerformanceGrade> grades)
        {
            int minGradeId = grades.Min(x => x.GradeId);
            int maxGradeId = grades.Max(x => x.GradeId);

            record.GradeId = cryptoService.Decrypt(record.GradeId);
            int.TryParse(record.GradeId, out int gradeId);

            if (gradeId > 0)
            {
                gradeId = gradeId + record.ToeicPolicy <= 0 ? minGradeId : gradeId + record.ToeicPolicy;
            }

            if (!grades.Any(x => x.GradeId == gradeId))
            {
                gradeId = gradeId > maxGradeId ? maxGradeId : 0;
            }

            record.GradeId = gradeId == 0 ? null : cryptoService.Encrypt(gradeId.ToString());
            record.OriginalGradeId = record.GradeId;
        }

        private void CalculateNullFinalPAGrade(EditPAGradeDto record, IEnumerable<PerformanceGrade> grades)
        {
            int minGradeId = grades.Min(x => x.GradeId);

            record.GradeId = cryptoService.Encrypt(minGradeId.ToString());
        }

        private async Task<Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>>> GetSettingToSendNotificationAsync(ReceiveNotificationType receiveType)
        {
            var criteria = new GetSettingToSendNotificationCriteria
            {
                ReceiveType = (int)receiveType
            };

            return await baseService.FetchMultipleAsync<PeriodModel, ProcessSettingModel>(criteria);
        }

        private async Task SendEmailAsync(string subject, string body, ProcessSettingModel setting)
        {
            var email = new MailModel
            {
                Subject = subject,
                Body = body,
                ReceiverEmail = setting.Email
            };

            await mailService.SendMailAsync(email);
        }
    }
}