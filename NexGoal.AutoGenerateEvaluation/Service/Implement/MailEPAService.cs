using NexGoal.Core.Enumerations;
using NexGoal.Criteria.Period;
using NexGoal.Criteria.Period.GenerationEvaluation;
using NexGoal.Model.Period;
using NexGoal.Services.Implement;
using NexGoal.Services.Interfaces;
using System.Collections.Generic;

namespace NexGoal.AutoGenerateEvaluation
{
    public class MailEPAService : IMailEPAService
    {
        private readonly IEvaluationService evaluationService;
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;

        public MailEPAService()
            : this(null, null, null)
        {
        }

        public MailEPAService(
            IEvaluationService evaluationService,
            IPeriodService periodService,
            IAutoMailService autoMailService)
        {
            this.evaluationService = evaluationService ?? new EvaluationService();
            this.periodService = periodService ?? new PeriodService();
            this.autoMailService = autoMailService ?? new AutoMailService();
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
            var criteria = new GetPeriodHasReminderProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> reminderProcess = evaluationService.GetPeriodHasReminderProcess(criteria);

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

        private void GetAndSendEmailToAllRelatedEmployee(
            int periodId,
            string employeeIds,
            int? processId,
            int mailMode)
        {
            PeriodModel period = GetPeriodInformation(periodId);
            IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds, processId);

            switch (mailMode)
            {
                case (int)MailMode.Start:
                    autoMailService.SendStartMail(period, employeeSettings);
                    break;

                case (int)MailMode.Reminder:
                    autoMailService.SendReminderMail(period, employeeSettings);
                    break;

                case (int)MailMode.EndPeriod:
                    autoMailService.SendEndEvaluationMailToHumanResourceByPeriod(period, employeeSettings);
                    break;
            }
        }

        private PeriodModel GetPeriodInformation(int periodId)
        {
            var period = new PeriodModel();

            var criteria = new GetPeriodByIdCriteria
            {
                PeriodId = periodId
            };
            period = periodService.GetPeriodById(criteria);

            return period;
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
    }
}
