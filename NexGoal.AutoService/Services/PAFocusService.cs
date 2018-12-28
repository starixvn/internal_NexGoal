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
    using NexGoal.Resources;

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
            var body = string.Empty;

            foreach (PeriodModel period in tuple.Item1)
            {
                var subjectStartMail = string.Format(ViewLang.EmailSubject_PAFocus_Start, period.Name);
                var subjectRemindMail = string.Format(ViewLang.EmailSubject_PAFocus_Remind, period.Name);

                foreach (ProcessSettingModel focusSetting in tuple.Item2)
                {
                    switch (focusSetting.MailMode)
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

        private IEnumerable<int> GetPeriodStarted()
        {
            var criteria = new GetPeriodStartedCriteria();

            return baseService.GetDataAsync<int>(criteria).Result;
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