namespace NexGoal.AutoService.Services
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Criterias.PAFocus;
    using NexGoal.AutoService.Models.PAFocus;
    using NexGoal.AutoService.Models.Period;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

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
            var criteria = new GetSettingToSendMailCriteria();
            var tuple = baseService.FetchMultipleAsync<PeriodModel, ProcessSettingModel>(criteria).Result;

            var path = Directory.GetCurrentDirectory();
            var startMailPath = Path.Combine(path, StartMailTemplateToEvaluator);
            var remindMailPath = Path.Combine(path, RemindMailTemplateToEvaluator);
            var subjectStartMail = "[NexGoal] PA Focus";
            var subjectRemindMail = "Reminder: [NexGoal] PA Focus";

            foreach (PeriodModel period in tuple.Item1)
            {
                foreach (ProcessSettingModel focusSetting in tuple.Item2)
                {
                    switch (focusSetting.MailMode)
                    {
                        case MailMode.Start:
                            SendEmail(subjectStartMail, startMailPath, period, focusSetting);
                            break;

                        case MailMode.Remind:
                            SendEmail(subjectRemindMail, remindMailPath, period, focusSetting);
                            break;

                        default:
                            continue;
                    }
                }
            }
        }

        private IEnumerable<int> GetPeriodStarted()
        {
            var criteria = new GetPeriodStartedCriteria();

            return baseService.GetDataAsync<int>(criteria).Result;
        }

        private void SendEmail(string subject, string templateEmailPath, PeriodModel period, ProcessSettingModel focusSetting)
        {
            var email = new MailModel();
            string startDate = mailService.FormatStartDate(period.FocusStartDate);
            string endDate = mailService.FormatStartDate(period.FocusEndDate);
            var body = string.Format(
                mailService.ReadTextFile(templateEmailPath),
                focusSetting.Nickname,
                period.Name,
                configuration.Value.ApplicationDomain,
                startDate,
                endDate);

            email.Subject = subject;
            email.Body = body;
            email.ReceiverEmail = focusSetting.Email;

            mailService.SendMailAsync(email).Wait();
        }
    }
}