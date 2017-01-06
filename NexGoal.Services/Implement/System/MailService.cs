namespace NexGoal.Services.Implement
{
    using Model;
    using NexGoal.Core;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Services;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.Period;
    using NexGoal.Resources;
    using NexGoal.Services.Interfaces;
    using System;
    using System.Configuration;

    public class MailService : IMailService
    {
        private readonly IBaseMailService _baseMailService;

        public MailService()
            : this(null)
        {
        }

        public MailService(IBaseMailService baseMailService)
        {
            _baseMailService = baseMailService ?? new BaseMailService();
        }

        public void SendMailToHRStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = ChangeStartDate(employeeSetting.StartDate.Value),
                    endDate = ChangeEndDate(employeeSetting.EndDate.Value);
                var body = _baseMailService.ReadTextFile(templateEmailPath)
                .Replace(EmailFormat.AdminNick, employeeSetting.EvaluatorInfo.NickName)
                .Replace(EmailFormat.PeriodName, period.Name)
                .Replace(EmailFormat.StartDate, startDate)
                .Replace(EmailFormat.EndDate, endDate)
                .Replace(EmailFormat.Url, AppSetting.ApplicationDomain);

                var subject = Email.StartDateEmailToHRSubject
                    .Replace(EmailFormat.PeriodName, period.Name)
                    .Replace(EmailFormat.TargetType, GetTargetType(period.TargetType.TargetTypeId));

                email.Subject = subject;
                email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
                email.Body = body;
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToSelfStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = ChangeStartDate(employeeSetting.StartDate.Value),
                    endDate = ChangeEndDate(employeeSetting.EndDate.Value);
                var body = _baseMailService.ReadTextFile(templateEmailPath)
                   .Replace(EmailFormat.EmployeeNick, employeeSetting.EvaluatorInfo.NickName)
                   .Replace(EmailFormat.PeriodName, period.Name)
                   .Replace(EmailFormat.StartDate, startDate)
                   .Replace(EmailFormat.EndDate, endDate)
                   .Replace(EmailFormat.Url, AppSetting.ApplicationDomain);

                var subject = Email.StartDateEmailToSelfSubject
                    .Replace(EmailFormat.PeriodName, period.Name)
                    .Replace(EmailFormat.TargetType, GetTargetType(period.TargetType.TargetTypeId));

                email.Subject = subject;
                email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
                email.Body = body;
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToEvaluatorStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = ChangeStartDate(employeeSetting.StartDate.Value),
                    endDate = ChangeEndDate(employeeSetting.EndDate.Value);

                var body = _baseMailService.ReadTextFile(templateEmailPath)
                    .Replace(EmailFormat.EvaluatorNick, employeeSetting.EvaluatorInfo.NickName)
                    .Replace(EmailFormat.PeriodName, period.Name)
                    .Replace(EmailFormat.StartDate, startDate)
                    .Replace(EmailFormat.EndDate, endDate)
                    .Replace(EmailFormat.Url, AppSetting.ApplicationDomain);

                var subject = Email.StartDateEmailToEvaluatorSubject
                    .Replace(EmailFormat.PeriodName, period.Name)
                    .Replace(EmailFormat.TargetType, GetTargetType(period.TargetType.TargetTypeId));

                email.Subject = subject;
                email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
                email.Body = body;
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToReviewerStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = ChangeStartDate(employeeSetting.StartDate.Value),
                    endDate = ChangeEndDate(employeeSetting.EndDate.Value);

                var body = _baseMailService.ReadTextFile(templateEmailPath)
                   .Replace(EmailFormat.ReviewerNick, employeeSetting.EvaluatorInfo.NickName)
                   .Replace(EmailFormat.PeriodName, period.Name)
                   .Replace(EmailFormat.StartDate, startDate)
                   .Replace(EmailFormat.EndDate, endDate)
                   .Replace(EmailFormat.Url, AppSetting.ApplicationDomain);
                var subject = Email.StartDateEmailToReviewerSubject
                    .Replace(EmailFormat.PeriodName, period.Name)
                    .Replace(EmailFormat.TargetType, GetTargetType(period.TargetType.TargetTypeId));

                email.Subject = subject;
                email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
                email.Body = body;
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToSelfRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = ChangeStartDate(employeeSetting.StartDate.Value),
                    endDate = ChangeEndDate(employeeSetting.EndDate.Value);

                var body = _baseMailService.ReadTextFile(templateEmailPath)
                    .Replace(EmailFormat.EmployeeNick, employeeSetting.EvaluatorInfo.NickName)
                    .Replace(EmailFormat.PeriodName, period.Name)
                    .Replace(EmailFormat.StartDate, startDate)
                    .Replace(EmailFormat.EndDate, endDate)
                    .Replace(EmailFormat.Url, AppSetting.ApplicationDomain);

                var subject = Email.RemindDateEmailToSelfSubject
                    .Replace(EmailFormat.PeriodName, period.Name)
                    .Replace(EmailFormat.TargetType, GetTargetType(period.TargetType.TargetTypeId));

                email.Subject = subject;
                email.Body = body;
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToEvaluatorRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = ChangeStartDate(employeeSetting.StartDate.Value),
                    endDate = ChangeEndDate(employeeSetting.EndDate.Value);

                var body = _baseMailService.ReadTextFile(templateEmailPath)
                    .Replace(EmailFormat.EvaluatorNick, employeeSetting.EvaluatorInfo.NickName)
                    .Replace(EmailFormat.PeriodName, period.Name)
                    .Replace(EmailFormat.StartDate, startDate)
                    .Replace(EmailFormat.EndDate, endDate)
                    .Replace(EmailFormat.Url, AppSetting.ApplicationDomain);

                var subject = Email.RemindDateEmailToEvaluatorSubject
                    .Replace(EmailFormat.PeriodName, period.Name)
                    .Replace(EmailFormat.TargetType, GetTargetType(period.TargetType.TargetTypeId));

                email.Subject = subject;
                email.Body = body;
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToReviewerRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = ChangeStartDate(employeeSetting.StartDate.Value),
                    endDate = ChangeEndDate(employeeSetting.EndDate.Value);

                var body = _baseMailService.ReadTextFile(templateEmailPath)
                   .Replace(EmailFormat.ReviewerNick, employeeSetting.EvaluatorInfo.NickName)
                   .Replace(EmailFormat.PeriodName, period.Name)
                   .Replace(EmailFormat.StartDate, startDate)
                   .Replace(EmailFormat.EndDate, endDate)
                   .Replace(EmailFormat.Url, AppSetting.ApplicationDomain);

                var subject = Email.RemindDateEmailToReviewerSubject
                    .Replace(EmailFormat.PeriodName, period.Name)
                    .Replace(EmailFormat.TargetType, GetTargetType(period.TargetType.TargetTypeId));

                email.Subject = subject;
                email.Body = body;
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToHRRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = ChangeStartDate(employeeSetting.StartDate.Value),
                    endDate = ChangeEndDate(employeeSetting.EndDate.Value);

                var body = _baseMailService.ReadTextFile(templateEmailPath)
                   .Replace(EmailFormat.AdminNick, employeeSetting.EvaluatorInfo.NickName)
                   .Replace(EmailFormat.PeriodName, period.Name)
                   .Replace(EmailFormat.StartDate, startDate)
                   .Replace(EmailFormat.EndDate, endDate)
                   .Replace(EmailFormat.Url, AppSetting.ApplicationDomain);

                var subject = Email.RemindDateEmailToHRSubject
                    .Replace(EmailFormat.PeriodName, period.Name)
                    .Replace(EmailFormat.TargetType, GetTargetType(period.TargetType.TargetTypeId));

                email.Subject = subject;
                email.Body = body;
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToNotifyChangeSetting(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();

                email.Subject = string.Format(Lang.NotifyEmailChangeSetting, periodName);
                email.Body = _baseMailService.ReadTextFile(templateEmailPath);
                email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, periodName);
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailEndToHRByEmployee(string templateEmailPath, EPAModel evaluationInfo, EPAProcessModel humanResource)
        {
            if (evaluationInfo != null && humanResource != null)
            {
                MailModel email = new MailModel();
                var body = _baseMailService.ReadTextFile(templateEmailPath)
                    .Replace(EmailFormat.AdminNick, humanResource.NickName)
                    .Replace(EmailFormat.PeriodName, evaluationInfo.Period.Name);

                var subject = Email.EndMailEvaluationSubject
                    .Replace(EmailFormat.PeriodName, evaluationInfo.Period.Name)
                    .Replace(EmailFormat.TargetType, GetTargetType(evaluationInfo.Period.TargetTypeId));

                email.Subject = subject;
                email.Body = body;
                email.ReceiverEmail = humanResource.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailEndToHRByPeriod(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                var body = _baseMailService.ReadTextFile(templateEmailPath)
                    .Replace(EmailFormat.PeriodName, period.Name)
                    .Replace(EmailFormat.AdminNick, employeeSetting.EvaluatorInfo.NickName);

                var subject = Email.EndMailEvaluationSubject
                    .Replace(EmailFormat.PeriodName, period.Name)
                    .Replace(EmailFormat.TargetType, GetTargetType(period.TargetType.TargetTypeId));

                email.Subject = subject;
                email.Body = body;
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToSelfEndEvaluation(string templateEmailPath, EPAModel evaluationInfo)
        {
            if (evaluationInfo != null)
            {
                MailModel email = new MailModel();
                email.Subject = Email.EndMailEvaluationSubject
                    .Replace(EmailFormat.PeriodName, evaluationInfo.Period.Name)
                    .Replace(EmailFormat.TargetType, GetTargetType(evaluationInfo.Period.TargetTypeId));

                email.Body = _baseMailService.ReadTextFile(templateEmailPath)
                    .Replace(EmailFormat.PeriodName, evaluationInfo.Period.Name)
                    .Replace(EmailFormat.EmployeeNick, evaluationInfo.Employee.NickName);

                email.ReceiverEmail = evaluationInfo.Employee.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public string ChangeStartDate(DateTime startDate)
        {
            var result = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, 0, startDate.Kind);
            return result.ToString(Constants.FormatDateTimeEmail);
        }

        public string ChangeEndDate(DateTime endDate)
        {
            var result = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 0, 0, endDate.Kind);
            return result.ToString(Constants.FormatDateTimeEmail);
        }

        public void SendStartMailToNominator(string templateEmailPath, EOQYModel nomination, EmailNominatorModel emailNomination)
        {
            MailModel email = new MailModel();
            var endDate = ChangeEndDate(nomination.EndDate);
            email.Subject = Email.StartDateEmailToAllInNominationSubject
                .Replace(EmailFormat.NominationName, nomination.Title);

            email.Body = _baseMailService.ReadTextFile(templateEmailPath)
                .Replace(EmailFormat.NominatorNick, emailNomination.NickName)
                .Replace(EmailFormat.Url, AppSetting.ApplicationDomain)
                .Replace(EmailFormat.EndDate, endDate);

            email.ReceiverEmail = emailNomination.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendRemindMailToNominator(string templateEmailPath, EOQYModel nomination, EmailNominatorModel emailNomination)
        {
            MailModel email = new MailModel();
            var endDate = ChangeEndDate(nomination.EndDate);
            email.Subject = Email.RemindDateEmailToAllInNominationSubject
                .Replace(EmailFormat.NominationName, nomination.Title);

            email.Body = _baseMailService.ReadTextFile(templateEmailPath)
                .Replace(EmailFormat.NominationName, nomination.Title)
                .Replace(EmailFormat.NominatorNick, emailNomination.NickName)
                .Replace(EmailFormat.Url, AppSetting.ApplicationDomain)
                .Replace(EmailFormat.EndDate, endDate);

            email.ReceiverEmail = emailNomination.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendStartMailToManagement(string templateEmailPath, EOQYModel nomination, EmailNominatorModel emailNomination)
        {
            MailModel email = new MailModel();
            var endDate = ChangeEndDate(nomination.EndDate);
            var startDate = ChangeEndDate(nomination.StartDate);

            email.Subject = Email.StartDateEmailToAllInNominationSubject
                .Replace(EmailFormat.NominationName, nomination.Title);

            email.Body = _baseMailService.ReadTextFile(templateEmailPath)
                .Replace(EmailFormat.NominationName, nomination.Title)
                .Replace(EmailFormat.NominatorNick, emailNomination.NickName)
                .Replace(EmailFormat.Url, AppSetting.ApplicationDomain)
                .Replace(EmailFormat.EndDate, endDate)
                .Replace(EmailFormat.StartDate, startDate);

            email.ReceiverEmail = emailNomination.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendEndMailToManagement(string templateEmailPath, EOQYModel nomination, EmailNominatorModel emailNomination)
        {
            MailModel email = new MailModel();
            var endDate = ChangeEndDate(nomination.EndDate);
            var startDate = ChangeEndDate(nomination.StartDate);

            email.Subject = Email.StartDateEmailToAllInNominationSubject
                .Replace(EmailFormat.NominationName, nomination.Title);

            email.Body = _baseMailService.ReadTextFile(templateEmailPath)
                .Replace(EmailFormat.NominationName, nomination.Title)
                .Replace(EmailFormat.NominatorNick, emailNomination.NickName)
                .Replace(EmailFormat.Url, AppSetting.ApplicationDomain)
                .Replace(EmailFormat.EndDate, endDate)
                .Replace(EmailFormat.StartDate, startDate);

            email.ReceiverEmail = emailNomination.Email;

            _baseMailService.SendEmail(email);
        }

        private string GetTargetType(int targetTypeId)
        {
            return targetTypeId == (int)TargetType.SemiAnnual ? Email.TargetTypePA : Email.TargetTypeProbation;
        }
    }
}