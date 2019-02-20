namespace NexGoal.AutoService.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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

        private readonly IOptionsSnapshot<AppSettings> configuration;
        private readonly IBaseService baseService;
        private readonly IBaseMailService mailService;
        private readonly ICryptoService cryptoService;

        public PAService(
            IOptionsSnapshot<AppSettings> configuration,
            IBaseService baseService,
            IBaseMailService mailService,
            ICryptoService cryptoService)
        {
            this.configuration = configuration;
            this.baseService = baseService;
            this.mailService = mailService;
            this.cryptoService = cryptoService;
        }

        public CommandResult EditGrade()
        {
            var commandResult = new CommandResult();
            IEnumerable<EditPAGradeDto> records = GetDataToEditGrade();

            if (records != null && records.Any())
            {
                var criteria = new EditGradeCriteria { Value = records.ToXML().ToString() };

                commandResult = baseService.ExecuteNonQueryAsync(criteria).Result;
            }

            return commandResult;
        }

        public CommandResult EditWhenEvaluationFinished()
        {
            var criteria = new EditWhenEvaluationFinishedCriteria();

            return baseService.ExecuteNonQueryAsync(criteria).Result;
        }

        public CommandResult EditStatus()
        {
            var criteria = new EditStatusCriteria();

            return baseService.ExecuteNonQueryAsync(criteria).Result;
        }

        public void SendMail()
        {
            Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>> tuple = GetSettingToSendNotification(ReceiveNotificationType.Email);

            var path = Directory.GetCurrentDirectory();
            var remindStartEvaluationToSelfPath = Path.Combine(path, RemindStartEvaluationMailTemplateToSelf);
            var remindStartEvaluationToSupervisorPath = Path.Combine(path, RemindStartEvaluationMailTemplateToSupervisor);
            var startToReviewerPath = Path.Combine(path, StartMailTemplateToReviewer);
            var remindToReviewerPath = Path.Combine(path, RemindMailTemplateToReviewer);
            var endEvaluationMailPath = Path.Combine(path, EndEvaluationMailTemplateToSelf);

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
                                period.Name,
                                mailService.FormatStartDate(period.StartDate),
                                configuration.Value.ApplicationDomain);
                            SendEmail(subjectRemindStartEvaluationMail, body, setting);
                            break;

                        case MailMode.Start:
                            body = string.Format(
                                mailService.ReadTextFile(startToReviewerPath),
                                setting.Nickname,
                                period.Name,
                                configuration.Value.ApplicationDomain);
                            SendEmail(subjectStartMail, body, setting);
                            break;

                        case MailMode.Remind:
                            body = string.Format(
                                mailService.ReadTextFile(remindToReviewerPath),
                                setting.Nickname,
                                period.Name,
                                mailService.FormatStartDate(setting.EndDate),
                                configuration.Value.ApplicationDomain);
                            SendEmail(subjectRemindMail, body, setting);
                            break;

                        case MailMode.EndEvaluation:
                            body = string.Format(
                                mailService.ReadTextFile(endEvaluationMailPath),
                                setting.Nickname,
                                period.Name,
                                configuration.Value.ApplicationDomain);
                            SendEmail(subjectEndEvaluationMail, body, setting);
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
                                    period.Name,
                                    mailService.FormatStartDate(period.StartDate)),
                                StatusId = (int)NotificationStatus.Unread,
                                ActionTypeId = (int)NotificationActionType.MyPageWithCreatePerformanceCheckin
                            });
                            break;

                        case MailMode.Start:
                            notifications.Add(new NotificationModel
                            {
                                EmployeeId = setting.EvaluatorId,
                                Content = string.Format(Message.NotificationTemplate_PA_StartToReviewer, period.Name),
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
                                    mailService.FormatEndDate(setting.EndDate)),
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

                baseService.ExecuteNonQueryAsync(criteria).Wait();
            }
        }

        private IEnumerable<EditPAGradeDto> GetDataToEditGrade()
        {
            var criteria = new GetDataToEditGradeCriteria();
            var records = baseService.GetDataAsync<EditPAGradeDto>(criteria).Result;
            var grades = baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNameCriteria { RefNames = "PerformanceGrade" }).Result;
            foreach (EditPAGradeDto record in records)
            {
                record.GradeId = cryptoService.Decrypt(record.GradeId);
                int.TryParse(record.GradeId, out int gradeId);
                gradeId += record.ToeicPolicy;

                if (!grades.Any(x => x.ResourceId == gradeId))
                {
                    gradeId = gradeId > grades.Max(x => x.ResourceId) ? grades.Max(x => x.ResourceId) : 0;
                }

                if (gradeId == 0 && record.StatusId == (int)PARecordStatus.Closed)
                {
                    gradeId = grades.Min(x => x.ResourceId);
                }

                record.GradeId = gradeId == 0 ? null : cryptoService.Encrypt(gradeId.ToString());
                record.OriginalGradeId = record.GradeId;
            }

            return records;
        }

        private Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>> GetSettingToSendNotification(ReceiveNotificationType receiveType)
        {
            var criteria = new GetSettingToSendNotificationCriteria
            {
                ReceiveType = (int)receiveType
            };

            return baseService.FetchMultipleAsync<PeriodModel, ProcessSettingModel>(criteria).Result;
        }

        private void SendEmail(string subject, string body, ProcessSettingModel setting)
        {
            var email = new MailModel
            {
                Subject = subject,
                Body = body,
                ReceiverEmail = setting.Email
            };

            mailService.SendMailAsync(email).Wait();
        }
    }
}