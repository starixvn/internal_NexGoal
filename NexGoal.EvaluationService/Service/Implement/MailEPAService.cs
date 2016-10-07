using NexGoal.Core.Enumerations;
using NexGoal.Core.Services;
using NexGoal.Criteria.Period;
using NexGoal.Criteria.Period.GenerationEvaluation;
using NexGoal.Model.Period;
using NexGoal.Services.Implement;
using NexGoal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NexGoal.BackgroundService
{
    public class MailEPAService : BaseService, IMailEPAService
    {
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;

        public MailEPAService()
            : this(null, null)
        {
        }

        public MailEPAService(
            IPeriodService periodService,
            IAutoMailService autoMailService)
        {
            this.periodService = periodService ?? new PeriodService();
            this.autoMailService = autoMailService ?? new AutoMailService();
        }

        public void CheckSendNoticeAtEndPeriod()
        {
            var criteria = new GetPeriodHasEndedProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> startedProcess = periodService.GetPeriodHasEndedProcess(criteria);

            foreach (AutoEmployeePeriodModel periodProcess in startedProcess)
            {
                GetAndSendEmailToAllRelatedEmployee(periodProcess.PeriodId, string.Empty, (int)ProcessType.HumanResource, (int)MailMode.EndPeriod);
            }
        }

        public void CheckSendNoticeAtBeginEmail()
        {
            var criteria = new GetPeriodHasStartedProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> startedProcess = periodService.GetPeriodHasStartedProcess(criteria);

            foreach (AutoEmployeePeriodModel periodProcess in startedProcess)
            {
                GetAndSendEmailToAllRelatedEmployee(periodProcess.PeriodId, string.Empty, periodProcess.ProcessId, (int)MailMode.Start);
            }
        }

        public void CheckSendReminderEmail()
        {
            IEnumerable<AutoEmployeePeriodModel> reminderProcess = GetPeriodHasReminderProcess();

            foreach (AutoEmployeePeriodModel periodProcess in reminderProcess)
            {
                if (periodProcess.TargetTypeId == (int)TargetType.SemiAnnual
                    && (periodProcess.ProcessId == (int)ProcessType.Self || periodProcess.ProcessId == (int)ProcessType.Evaluator)
                    && periodProcess.HasMonthlyEPA)
                {
                    continue;
                }
                else
                {
                    GetAndSendEmailToAllRelatedEmployee(periodProcess.PeriodId, string.Empty, periodProcess.ProcessId, (int)MailMode.Reminder);
                }
            }
        }

        private IEnumerable<AutoEmployeePeriodModel> GetPeriodHasReminderProcess()
        {
            IEnumerable<AutoEmployeePeriodModel> reminderPeriods = new List<AutoEmployeePeriodModel>();
            var criteria = new GetPeriodHasReminderProcessCriteria();
            if (criteria != null && criteria.IsValid())
            {
                reminderPeriods = GetData<AutoEmployeePeriodModel>(criteria);
            }

            return reminderPeriods;
        }

        private void GetAndSendEmailToAllRelatedEmployee(
            int periodId,
            string employeeIds,
            int? processId,
            int mailMode)
        {
            PeriodModel period = periodService.GetPeriodById(periodId);
            IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds, processId);

            switch (mailMode)
            {
                case (int)MailMode.Start:
                    employeeSettings = employeeSettings.Where(i => i.StartDate.Value.Date == DateTime.Now.Date);
                    autoMailService.SendStartMail(period, employeeSettings);
                    break;

                case (int)MailMode.Reminder:
                    employeeSettings = employeeSettings.Where(i => i.RemindDate.Value.Date == DateTime.Now.Date && i.StartDate.Value.Date <= DateTime.Now.Date);
                    autoMailService.SendReminderMail(period, employeeSettings);
                    break;

                case (int)MailMode.EndPeriod:
                    employeeSettings = GetEmployeeSettingOfHR(periodId);
                    autoMailService.SendEndEvaluationMailToHRByPeriod(period, employeeSettings);
                    break;
            }
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId, string employeeIds, int? processId)
        {
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();

            var getEmployeeSettingCriteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds,
                ProcessId = processId
            };
            employeeSettings = periodService.GetEmployeeSettingByPeriodId(getEmployeeSettingCriteria);

            return employeeSettings;
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettingOfHR(int periodId)
        {
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();

            var getEmployeeSettingCriteria = new GetEmployeeSettingOfHRCriteria
            {
                PeriodId = periodId
            };
            employeeSettings = periodService.GetEmployeeSettingOfHR(getEmployeeSettingCriteria);

            return employeeSettings;
        }
    }
}
