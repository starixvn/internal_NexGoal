namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using NexGoal.Core.Enumerations;
    using NexGoal.Model.Period;
    using NexGoal.Services.Interfaces;

    public class ManualMailService : IManualMailService
    {
        private readonly string _startDateMailToHumanResources = "~/Views/Emails/StartEvaluationToHR.cshtml";
        private readonly string _startDateMailToSelf = "~/Views/Emails/StartEvaluationToSelf.cshtml";
        private readonly string _startDateMailToEvaluator = "~/Views/Emails/StartEvaluationToEvaluator.cshtml";
        private readonly string _startDateMailToReviewer = "~/Views/Emails/StartEvaluationToReviewer.cshtml";
        private readonly string _mailNotifyChangeSetting = "~/Views/Emails/EmailNotifyChangeSetting.cshtml";

        private readonly IMailService _mailService;

        public ManualMailService()
            : this(null)
        {
        }

        public ManualMailService(IMailService mailService)
        {
            _mailService = mailService ?? new MailService();
        }

        public void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                string templateEmailPath = string.Empty;

                foreach (PeriodFlowModel employeeSetting in employeeSettings)
                {
                    switch (employeeSetting.ProcessId)
                    {
                        case (int)ProcessType.HumanResource:
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToHumanResources);
                            _mailService.SendMailToHRStartDate(templateEmailPath, employeeSetting, period);
                            break;

                        case (int)ProcessType.Self:
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToSelf);
                            _mailService.SendMailToSelfStartDate(templateEmailPath, employeeSetting, period);
                            break;

                        case (int)ProcessType.Evaluator:
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToEvaluator);
                            _mailService.SendMailToEvaluatorStartDate(templateEmailPath, employeeSetting, period);

                            break;

                        case (int)ProcessType.Reviewer:
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToReviewer);
                            _mailService.SendMailToReviewerStartDate(templateEmailPath, employeeSetting, period);

                            break;
                    }
                }
            }
        }

        public void SendNotifyMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                string templateEmailPath = HttpContext.Current.Server.MapPath(_mailNotifyChangeSetting);

                foreach (PeriodFlowModel employeeSetting in employeeSettings)
                {
                    _mailService.SendMailToNotifyChangeSetting(templateEmailPath, employeeSetting, period.Name);
                }
            }
        }
    }
}