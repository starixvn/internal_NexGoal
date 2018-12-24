namespace NexGoal.AutoService.Services
{
    using System.IO;
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Criterias.Nomination;
    using NexGoal.AutoService.Models.Nomination;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class NominationService : INominationService
    {
        private const string StartMailTemplateToNominator = "EmailTemplates\\Nomination\\StartToNominator.cshtml";
        private const string RemindMailTemplateToNominator = "EmailTemplates\\Nomination\\RemindToNominator.cshtml";

        private readonly IBaseService baseService;
        private readonly IBaseMailService mailService;
        private readonly IOptionsSnapshot<AppSettings> configuration;

        public NominationService(
            IOptionsSnapshot<AppSettings> configuration,
            IBaseService baseService,
            IBaseMailService mailService)
        {
            this.configuration = configuration;
            this.baseService = baseService;
            this.mailService = mailService;
        }

        public void SendMail()
        {
            var criteria = new GetSettingToSendMailCriteria();
            var tuple = baseService.FetchMultipleAsync<NominationPeriodModel, ProcessSettingModel>(criteria).Result;

            var path = Directory.GetCurrentDirectory();
            var startMailPath = Path.Combine(path, StartMailTemplateToNominator);
            var remindMailPath = Path.Combine(path, RemindMailTemplateToNominator);

            foreach (NominationPeriodModel period in tuple.Item1)
            {
                var subjectStartMail = string.Format("[NexGoal] {0} - Nomination", period.Title);
                var subjectRemindMail = string.Format("Reminder: [NexGoal] {0} - Nomination", period.Title);
                foreach (ProcessSettingModel setting in tuple.Item2)
                {
                    switch (setting.MailMode)
                    {
                        case MailMode.Start:
                            SendEmail(subjectStartMail, startMailPath, period, setting);
                            break;

                        case MailMode.Remind:
                            SendEmail(subjectRemindMail, remindMailPath, period, setting);
                            break;

                        default:
                            continue;
                    }
                }
            }
        }

        public CommandResult EditWhenFinished()
        {
            var criteria = new EditWhenFinishedCriteria();

            return baseService.ExecuteNonQueryAsync(criteria).Result;
        }

        private void SendEmail(string subject, string templateEmailPath, NominationPeriodModel period, ProcessSettingModel setting)
        {
            var email = new MailModel();
            string endDate = mailService.FormatStartDate(period.EndDateForNominator);
            var body = string.Format(
                mailService.ReadTextFile(templateEmailPath),
                setting.Nickname,
                configuration.Value.ApplicationDomain,
                endDate);

            email.Subject = subject;
            email.Body = body;
            email.ReceiverEmail = setting.Email;

            mailService.SendMailAsync(email).Wait();
        }
    }
}