namespace NexGoal.BackgroundService
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using Core;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Services;
    using Criteria.PAFocus;
    using Model.PAFocus;

    public class PAFocusAutoService : BaseService, IPAFocusAutoService
    {
        private readonly string startMailToEvaluator = "Views\\Emails\\PAFocus\\StartToEvaluator.cshtml";
        private readonly string reminderMailToEvaluator = "Views\\Emails\\PAFocus\\ReminderToEvaluator.cshtml";

        private readonly IBaseMailService baseMailService;

        public PAFocusAutoService() : this(null)
        {
        }

        public PAFocusAutoService(IBaseMailService baseMailService)
        {
            this.baseMailService = baseMailService ?? new BaseMailService();
        }

        public void Run()
        {
            IEnumerable<PAFocusMailModel> focusSettings = GetProcessSettingToSendEmail();
            SendEmail(focusSettings);
        }

        private IEnumerable<PAFocusMailModel> GetProcessSettingToSendEmail()
        {
            IEnumerable<PAFocusMailModel> result = new List<PAFocusMailModel>();
            var criteria = new GetProcessSettingToSendEmailCriteria();

            if (criteria.IsValid())
            {
                result = GetData<PAFocusMailModel>(criteria);
            }

            return result;
        }

        private void SendEmail(IEnumerable<PAFocusMailModel> focusSettings)
        {
            var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            var templateStartEmailPath = Path.Combine(assemblyPath, startMailToEvaluator);
            var templateReminderEmailPath = Path.Combine(assemblyPath, reminderMailToEvaluator);
            var subjectStartMail = "[NexGoal] PA Focus";
            var subjectReminderMail = "Reminder: [NexGoal] PA Focus";

            foreach (var focusSetting in focusSettings)
            {
                switch (focusSetting.MailMode)
                {
                    case (int)MailMode.Start:
                        SendEmail(subjectStartMail, templateStartEmailPath, focusSetting);
                        break;

                    case (int)MailMode.Reminder:
                        SendEmail(subjectReminderMail, templateReminderEmailPath, focusSetting);
                        break;

                    default:
                        continue;
                }
            }
        }

        private void SendEmail(string subject, string templateEmailPath, PAFocusMailModel focusSetting)
        {
            var email = new MailModel();
            string startDate = baseMailService.ChangeStartDate(focusSetting.StartDate);
            string endDate = baseMailService.ChangeEndDate(focusSetting.EndDate);
            var body = baseMailService.ReadTextFile(templateEmailPath)
                .Replace(EmailFormat.EvaluatorNick, focusSetting.NickName)
                .Replace(EmailFormat.PeriodName, focusSetting.PeriodName)
                .Replace(EmailFormat.StartDate, startDate)
                .Replace(EmailFormat.EndDate, endDate)
                .Replace(EmailFormat.Url, Core.AppSetting.ApplicationDomain);

            email.Subject = subject;
            email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
            email.Body = body;
            email.ReceiverEmail = focusSetting.Email;

            baseMailService.SendEmail(email);
        }
    }
}