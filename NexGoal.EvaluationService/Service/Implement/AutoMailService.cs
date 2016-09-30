namespace NexGoal.BackgroundService
{
    using NexGoal.Core.Enumerations;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.Period;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class AutoMailService : IAutoMailService
    {
        private string _startDateMailToHumanResources = "Views\\Emails\\StartDateEmailToHR.cshtml";
        private string _startDateMailToSelf = "Views\\Emails\\StartDateEmailToSelf.cshtml";
        private string _startDateMailToEvaluator = "Views\\Emails\\StartDateEmailToEvaluator.cshtml";
        private string _startDateMailToReviewer = "Views\\Emails\\StartDateEmailToReviewer.cshtml";

        private string _reminderMailToSelf = "Views\\Emails\\RemindDateEmailToSelf.cshtml";
        private string _reminderMailToEvaluator = "Views\\Emails\\RemindDateEmailToEvaluator.cshtml";
        private string _reminderMailToReviewer = "Views\\Emails\\RemindDateEmailToReviewer.cshtml";
        private string _reminderMailToHR = "Views\\Emails\\RemindDateEmailToHR.cshtml";

        private string _endMailToHumanResourcesByEmployee = "Views\\Emails\\EndEvaluationEmailToHRByEmployee.cshtml";
        private string _endMailToHumanReouscesByPeriod = "Views\\Emails\\EndEvaluationEmailToHRByPeriod.cshtml";

        private IMailService _mailService;

        public AutoMailService()
            : this(null)
        {
        }

        public AutoMailService(IMailService mailService)
        {
            _mailService = mailService ?? new MailService();
        }

        public void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                string templateEmailPath = string.Empty,
                    assemblyPath = AppDomain.CurrentDomain.BaseDirectory;

                foreach (PeriodFlowModel employeeSetting in employeeSettings.Where(x => x.HasSendMail))
                {
                    switch (employeeSetting.ProcessId)
                    {
                        case (int)ProcessType.HumanResource:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToHumanResources);
                            _mailService.SendMailToHRStartDate(templateEmailPath, employeeSetting, period);
                            break;

                        case (int)ProcessType.Self:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToSelf);
                            _mailService.SendMailToSelfStartDate(templateEmailPath, employeeSetting, period);
                            break;

                        case (int)ProcessType.Evaluator:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToEvaluator);
                            _mailService.SendMailToEvaluatorStartDate(templateEmailPath, employeeSetting, period);
                            break;

                        case (int)ProcessType.Reviewer:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToReviewer);
                            _mailService.SendMailToReviewerStartDate(templateEmailPath, employeeSetting, period);
                            break;
                    }
                }
            }
        }

        public void SendReminderMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                string templateEmailPath = string.Empty,
                    assemblyPath = AppDomain.CurrentDomain.BaseDirectory;

                foreach (PeriodFlowModel employeeSetting in employeeSettings)
                {
                    switch (employeeSetting.ProcessId)
                    {
                        case (int)ProcessType.Self:
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToSelf);
                            _mailService.SendMailToSelfRemindDate(templateEmailPath, employeeSetting, period);
                            break;

                        case (int)ProcessType.Evaluator:
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToEvaluator);
                            _mailService.SendMailToEvaluatorRemindDate(templateEmailPath, employeeSetting, period);

                            break;

                        case (int)ProcessType.Reviewer:
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToReviewer);
                            _mailService.SendMailToReviewerRemindDate(templateEmailPath, employeeSetting, period);

                            break;

                        case (int)ProcessType.HumanResource:
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToHR);
                            _mailService.SendMailToHRRemindDate(templateEmailPath, employeeSetting, period);

                            break;
                    }
                }
            }
        }

        public void SendEndEvaluationMailToHumanResourceByEmployee(EPAModel evaluationInfo)
        {
            if (evaluationInfo != null)
            {
                string assemblyPath = AppDomain.CurrentDomain.BaseDirectory,
                    templateEmailPath = Path.Combine(assemblyPath, _endMailToHumanResourcesByEmployee);

                foreach (EPAProcessModel humanResource in evaluationInfo.Process.HumanResources)
                {
                    _mailService.SendMailEndToHumanResourcesByEmployee(templateEmailPath, evaluationInfo, humanResource);
                }
            }
        }

        public void SendEndEvaluationMailToHumanResourceByPeriod(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                string assemblyPath = AppDomain.CurrentDomain.BaseDirectory,
                    templateEmailPath = Path.Combine(assemblyPath, _endMailToHumanReouscesByPeriod);

                foreach (PeriodFlowModel employeeSetting in employeeSettings)
                {
                    _mailService.SendMailEndToHumanResourceByPeriod(templateEmailPath, employeeSetting, period.Name);
                }
            }
        }
    }
}