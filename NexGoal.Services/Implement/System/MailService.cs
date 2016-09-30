namespace NexGoal.Services.Implement
{
    using NexGoal.Core;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Services;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.Period;
    using NexGoal.Resources;
    using NexGoal.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

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
                .Replace(EmailFormat.Duration, ((employeeSetting.EndDate.Value - employeeSetting.StartDate.Value).Days + 1).ToString())
                .Replace(EmailFormat.URL, AppSetting.ApplicationDomain);

                var subject = Email.StartDateEmailToHRSubject
                .Replace(EmailFormat.StartMonthYear, employeeSetting.StartDate.Value.ToString("MMMM yyyy"))
                .Replace(EmailFormat.TargetType, period.TargetType.TargetTypeId == (int)TargetType.SemiAnnual ? Email.TargetTypeSemi : Email.TargetTypeProbationForHR);

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
                   .Replace(EmailFormat.Duration, ((employeeSetting.EndDate.Value - employeeSetting.StartDate.Value).Days + 1).ToString())
                   .Replace(EmailFormat.URL, AppSetting.ApplicationDomain);

                var subject = Email.StartDateEmailToSelfSubject
                  .Replace(EmailFormat.StartMonthYear, employeeSetting.StartDate.Value.ToString("MMMM yyyy"))
                  .Replace(EmailFormat.TargetType, period.TargetType.TargetTypeId == (int)TargetType.SemiAnnual ? Email.TargetTypeSemi : Email.TargetTypeProbation);

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
                    .Replace(EmailFormat.Duration, ((employeeSetting.EndDate.Value - employeeSetting.StartDate.Value).Days + 1).ToString())
                    .Replace(EmailFormat.URL, AppSetting.ApplicationDomain);

                var subject = Email.StartDateEmailToEvaluatorSubject
                    .Replace(EmailFormat.StartMonthYear, employeeSetting.StartDate.Value.ToString("MMMM yyyy"))
                    .Replace(EmailFormat.TargetType, period.TargetType.TargetTypeId == (int)TargetType.SemiAnnual ? Email.TargetTypeSemi : Email.TargetTypeProbation);

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
                   .Replace(EmailFormat.Duration, ((employeeSetting.EndDate.Value - employeeSetting.StartDate.Value).Days + 1).ToString())
                   .Replace(EmailFormat.URL, AppSetting.ApplicationDomain);
                var subject = Email.StartDateEmailToEvaluatorSubject
                    .Replace(EmailFormat.StartMonthYear, employeeSetting.StartDate.Value.ToString("MMMM yyyy"))
                    .Replace(EmailFormat.TargetType, period.TargetType.TargetTypeId == (int)TargetType.SemiAnnual ? Email.TargetTypeSemi : Email.TargetTypeProbation);

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
                    .Replace(EmailFormat.Duration, ((employeeSetting.EndDate.Value - employeeSetting.StartDate.Value).Days + 1).ToString())
                    .Replace(EmailFormat.URL, AppSetting.ApplicationDomain);

                var subject = Email.RemindDateEmailToSelfSubject
                  .Replace(EmailFormat.StartMonthYear, employeeSetting.StartDate.Value.ToString("MMMM yyyy"))
                  .Replace(EmailFormat.TargetType, period.TargetType.TargetTypeId == (int)TargetType.SemiAnnual ? Email.TargetTypeSemi : Email.TargetTypeProbation);

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
                    .Replace(EmailFormat.Duration, ((employeeSetting.EndDate.Value - employeeSetting.StartDate.Value).Days + 1).ToString())
                    .Replace(EmailFormat.URL, AppSetting.ApplicationDomain);

                var subject = Email.RemindDateEmailToEvaluatorSubject
                    .Replace(EmailFormat.StartMonthYear, employeeSetting.StartDate.Value.ToString("MMMM yyyy"))
                    .Replace(EmailFormat.TargetType, period.TargetType.TargetTypeId == (int)TargetType.SemiAnnual ? Email.TargetTypeSemi : Email.TargetTypeProbation);

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
                   .Replace(EmailFormat.Duration, ((employeeSetting.EndDate.Value - employeeSetting.StartDate.Value).Days + 1).ToString())
                   .Replace(EmailFormat.URL, AppSetting.ApplicationDomain);

                var subject = Email.RemindDateEmailToReviewerSubject
                    .Replace(EmailFormat.StartMonthYear, employeeSetting.StartDate.Value.ToString("MMMM yyyy"))
                    .Replace(EmailFormat.TargetType, period.TargetType.TargetTypeId == (int)TargetType.SemiAnnual ? Email.TargetTypeSemi : Email.TargetTypeProbation);

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
                   .Replace(EmailFormat.Duration, ((employeeSetting.EndDate.Value - employeeSetting.StartDate.Value).Days + 1).ToString())
                   .Replace(EmailFormat.URL, AppSetting.ApplicationDomain);

                var subject = Email.RemindDateEmailToHRSubject
                  .Replace(EmailFormat.StartMonthYear, employeeSetting.StartDate.Value.ToString("MMMM yyyy"))
                  .Replace(EmailFormat.TargetType, period.TargetType.TargetTypeId == (int)TargetType.SemiAnnual ? Email.TargetTypeSemi : Email.TargetTypeProbationForHR);

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

        public void SendMailEndToHumanResourcesByEmployee(
            string templateEmailPath,
            EPAModel evaluationInfo,
            EPAProcessModel humanResource)
        {
            if (evaluationInfo != null && humanResource != null)
            {
                MailModel email = new MailModel();
                var body = _baseMailService.ReadTextFile(templateEmailPath)
                    .Replace(EmailFormat.AdminNick, humanResource.NickName)
                    .Replace(EmailFormat.EmployeeNick, evaluationInfo.Employee.NickName)
                    .Replace(EmailFormat.EmployeeCode, evaluationInfo.Employee.Code)
                    .Replace(EmailFormat.URL, AppSetting.ApplicationDomain)
                    .Replace(EmailFormat.ConfirmedDate, evaluationInfo.Employee.ConfirmedDate.ToString("dd MMM yyyy"));

                var subject = Lang.EndMailEvaluationToSelf
                    .Replace(EmailFormat.StartMonthYear, evaluationInfo.Period.StartDate.Value.ToString("MMMM yyyy"));

                email.Subject = subject;
                email.Body = body;
                email.ReceiverEmail = humanResource.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailEndToHumanResourceByPeriod(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                var body = _baseMailService.ReadTextFile(templateEmailPath)
                   .Replace(EmailFormat.AdminNick, employeeSetting.EvaluatorInfo.NickName)
                   .Replace(EmailFormat.URL, AppSetting.ApplicationDomain);

                var subject = Lang.EndMailEvaluationToSelf
                    .Replace(EmailFormat.StartMonthYear, employeeSetting.StartDate.Value.ToString("MMMM yyyy"));

                email.Subject = subject;
                email.Body = body;
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

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

        private string GetEvaluator(IList<EPAProcessModel> evaluators)
        {
            string names = string.Join(",", evaluators.Select(x => x.NickName));

            return names;
        }

        private float GetAdjustmentScore(EPAModel evaluationInfo, float averagePerformanceScore)
        {
            float adjustScore = 0;

            if (evaluationInfo.Process.AverageScoreByManagements > 0)
            {
                if (evaluationInfo.Process.AverageScoreByReviewers > 0)
                {
                    adjustScore = averagePerformanceScore - evaluationInfo.Process.AverageScoreByReviewers;
                }
                else
                {
                    adjustScore = averagePerformanceScore - evaluationInfo.Process.AverageScoreByEvaluators;
                }
            }
            else if (evaluationInfo.Process.AverageScoreByReviewers > 0)
            {
                adjustScore = averagePerformanceScore - evaluationInfo.Process.AverageScoreByEvaluators;
            }

            return adjustScore;
        }
    }
}