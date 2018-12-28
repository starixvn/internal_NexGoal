namespace NexGoal.AutoService.Services
{
    using System.IO;
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Criterias.Nomination;
    using NexGoal.AutoService.Models.Nomination;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Resources;

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
            var body = string.Empty;

            foreach (NominationPeriodModel period in tuple.Item1)
            {
                var subjectStartMail = string.Format(ViewLang.EmailSubject_Nomination_Start, period.Title);
                var subjectRemindMail = string.Format(ViewLang.EmailSubject_Nomination_Remind, period.Title);
                foreach (ProcessSettingModel setting in tuple.Item2)
                {
                    switch (setting.MailMode)
                    {
                        case MailMode.Start:
                            body = string.Format(
                                mailService.ReadTextFile(startMailPath),
                                setting.Nickname,
                                period.Title,
                                configuration.Value.ApplicationDomain);
                            SendEmail(subjectStartMail, body, setting.Email);
                            break;

                        case MailMode.Remind:
                            body = string.Format(
                                mailService.ReadTextFile(remindMailPath),
                                setting.Nickname,
                                period.Title,
                                mailService.FormatStartDate(period.EndDateForNominator),
                                configuration.Value.ApplicationDomain);
                            SendEmail(subjectRemindMail, body, setting.Email);
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