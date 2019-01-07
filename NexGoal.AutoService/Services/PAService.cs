namespace NexGoal.AutoService.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Criterias;
    using NexGoal.AutoService.Criterias.PA;
    using NexGoal.AutoService.Models;
    using NexGoal.AutoService.Models.PA;
    using NexGoal.AutoService.Models.Period;
    using NexGoal.Core.Enumerations;
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
            IEnumerable<PARecordModel> records = GetDataToEditGrade();

            if (records != null && records.Any())
            {
                var xmlTemplate = "<Row PARecordId='{0}' GradeId='{1}' ToeicPolicy='{2}' />";
                var xml = new StringBuilder();
                xml.Append("<Root>");

                foreach (PARecordModel record in records)
                {
                    xml.AppendFormat(xmlTemplate, record.PARecordId, record.GradeId, record.ToeicPolicy);
                }

                xml.Append("</Root>");

                var criteria = new EditGradeCriteria { Value = xml.ToString() };

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
            var criteria = new GetSettingToSendMailCriteria();
            var tuple = baseService.FetchMultipleAsync<PeriodModel, ProcessSettingModel>(criteria).Result;

            var path = Directory.GetCurrentDirectory();
            var remindStartEvaluationToSelfPath = Path.Combine(path, RemindStartEvaluationMailTemplateToSelf);
            var remindStartEvaluationToSupervisorPath = Path.Combine(path, RemindStartEvaluationMailTemplateToSupervisor);
            var startToReviewerPath = Path.Combine(path, StartMailTemplateToReviewer);
            var remindToReviewerPath = Path.Combine(path, RemindMailTemplateToReviewer);
            var endEvaluationMailPath = Path.Combine(path, EndEvaluationMailTemplateToSelf);

            foreach (PeriodModel period in tuple.Item1)
            {
                var subjectRemindStartEvaluationMail = string.Format(ViewLang.EmailSubject_PA_RemindStartEvaluation, period.Name);
                var subjectStartMail = string.Format(ViewLang.EmailSubject_PA_StartToReviewer, period.Name);
                var subjectRemindMail = string.Format(ViewLang.EmailSubject_PA_RemindToReviewer, period.Name);
                var subjectEndEvaluationMail = string.Format(ViewLang.EmailSubject_PA_EndEvaluation, period.Name);
                var body = string.Empty;

                foreach (ProcessSettingModel setting in tuple.Item2)
                {
                    switch (setting.MailMode)
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
                                configuration.Value.ApplicationDomain,
                                mailService.FormatStartDate(setting.StartDate),
                                mailService.FormatStartDate(setting.EndDate));
                            SendEmail(subjectStartMail, body, setting);
                            break;

                        case MailMode.Remind:
                            body = string.Format(
                                mailService.ReadTextFile(remindToReviewerPath),
                                setting.Nickname,
                                configuration.Value.ApplicationDomain,
                                mailService.FormatStartDate(setting.StartDate),
                                mailService.FormatStartDate(setting.EndDate));
                            SendEmail(subjectRemindMail, body, setting);
                            break;

                        case MailMode.EndEvaluation:
                            body = string.Format(
                                mailService.ReadTextFile(endEvaluationMailPath),
                                setting.Nickname,
                                period.Name);
                            SendEmail(subjectEndEvaluationMail, body, setting);
                            break;

                        default:
                            continue;
                    }
                }
            }
        }

        private IEnumerable<PARecordModel> GetDataToEditGrade()
        {
            var criteria = new GetDataToEditGradeCriteria();
            var records = baseService.GetDataAsync<PARecordModel>(criteria).Result;
            var grades = baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNameCriteria { RefNames = "PerformanceGrade" }).Result;
            foreach (PARecordModel record in records)
            {
                record.GradeId = cryptoService.Decrypt(record.GradeId);
                int.TryParse(record.GradeId, out int gradeId);
                gradeId += record.ToeicPolicy;

                if (!grades.Any(x => x.ResourceId == gradeId))
                {
                    gradeId = gradeId > grades.Max(x => x.ResourceId) ? grades.Max(x => x.ResourceId) : 0;
                }

                record.GradeId = gradeId == 0 ? null : cryptoService.Encrypt(gradeId.ToString());
            }

            return records;
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