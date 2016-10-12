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
        private string _startDateMailToHumanResources = "Views\\Emails\\StartEvaluationToHR.cshtml";
        private string _startDateMailToSelf = "Views\\Emails\\StartEvaluationToSelf.cshtml";
        private string _startDateMailToEvaluator = "Views\\Emails\\StartEvaluationToEvaluator.cshtml";
        private string _startDateMailToReviewer = "Views\\Emails\\StartEvaluationToReviewer.cshtml";

        private string _endMailToHumanResourcesByEmployee = "Views\\Emails\\EndEvaluationToHRByEmployee.cshtml";
        private string _endMailToHumanReouscesByPeriod = "Views\\Emails\\EndEvaluationToHRByPeriod.cshtml";
        private string _endMailToSelf = "Views\\Emails\\EndEvaluationToSelf.cshtml";

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
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToSelf);
                            _mailService.SendMailToSelfRemindDate(templateEmailPath, employeeSetting, period);
                            break;

                        case (int)ProcessType.Evaluator:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToEvaluator);
                            _mailService.SendMailToEvaluatorRemindDate(templateEmailPath, employeeSetting, period);

                            break;

                        case (int)ProcessType.Reviewer:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToReviewer);
                            _mailService.SendMailToReviewerRemindDate(templateEmailPath, employeeSetting, period);

                            break;

                        case (int)ProcessType.HumanResource:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToHumanResources);
                            _mailService.SendMailToHRRemindDate(templateEmailPath, employeeSetting, period);

                            break;
                    }
                }
            }
        }

        public void SendEndEvaluationMailToHRByEmployee(EPAModel evaluationInfo)
        {
            if (evaluationInfo != null)
            {
                string assemblyPath = AppDomain.CurrentDomain.BaseDirectory,
                    templateEmailPath = Path.Combine(assemblyPath, _endMailToHumanResourcesByEmployee);

                foreach (EPAProcessModel humanResource in evaluationInfo.Process.HumanResources)
                {
                    _mailService.SendMailEndToHRByEmployee(templateEmailPath, evaluationInfo, humanResource);
                }
            }
        }

        public void SendEndEvaluationMailToHRByPeriod(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                string assemblyPath = AppDomain.CurrentDomain.BaseDirectory,
                    templateEmailPath = Path.Combine(assemblyPath, _endMailToHumanReouscesByPeriod);
                foreach (PeriodFlowModel employeeSetting in employeeSettings)
                {
                    _mailService.SendMailEndToHRByPeriod(templateEmailPath, employeeSetting, period);
                }
            }
        }

        public void SendEndEvaluationMailToSelf(EPAModel evaluationInfo)
        {
            string assemblyPath = AppDomain.CurrentDomain.BaseDirectory,
                templateEmailPath = Path.Combine(assemblyPath, _endMailToSelf);

            _mailService.SendMailToSelfEndEvaluation(templateEmailPath, evaluationInfo);
        }
    }
}