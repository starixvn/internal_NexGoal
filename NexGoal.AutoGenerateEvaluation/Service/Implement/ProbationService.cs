using NexGoal.Core.Model;
using NexGoal.Criteria.Period;
using NexGoal.Criteria.Period.GenerationEvaluation;
using NexGoal.Model.Period;
using NexGoal.Services.Implement;
using NexGoal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexGoal.AutoGenerateEvaluation
{
    public class ProbationService : IProbationService
    {
        private readonly IEvaluationService evaluationService;
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;
        private readonly IEmployeeEPAService employeeEPAService;

        public ProbationService()
            : this(null, null, null, null)
        {
        }

        public ProbationService(
            IEvaluationService evaluationService,
            IPeriodService periodService,
            IAutoMailService autoMailService,
            IEmployeeEPAService employeeEPAService)
        {
            this.evaluationService = evaluationService ?? new EvaluationService();
            this.periodService = periodService ?? new PeriodService();
            this.autoMailService = autoMailService ?? new AutoMailService();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
        }

        public void CheckProbationPeriod()
        {
            var criteria = new GetEmployeeProbationEvaluationCriteria();
            AutoEmployeePeriodModel employeePeriod = evaluationService.GetEmployeeProbationEvaluation(criteria);
            if (!string.IsNullOrWhiteSpace(employeePeriod.EmployeeIds) && employeePeriod.PeriodId > 0)
            {
                AddEmployeeAndGenerateEPAProbation(employeePeriod);
            }
        }

        private void AddEmployeeAndGenerateEPAProbation(AutoEmployeePeriodModel employeePeriod)
        {
            var insertEmployeeCriteria = new InsertEmployeeToProbationPeriodCriteria
            {
                PeriodId = employeePeriod.PeriodId,
                EmployeeIds = employeePeriod.EmployeeIds
            };

            CommandResult insertEmployeeResult = evaluationService.InsertEmployeeToProbationPeriod(insertEmployeeCriteria);

            var generateEvaluationCriteria = new AutoGenerateEvaluationCriteria
            {
                PeriodId = employeePeriod.PeriodId,
                EmployeeIds = employeePeriod.EmployeeIds
            };
            CommandResult generateEvaluationResult = periodService.GenerateEvaluation(generateEvaluationCriteria);
            if (AppSetting.SendMailAllWhenPeriodStarted)
            {
                Task.Factory.StartNew(() =>
                {
                    GetAndSendEmailToAllRelatedEmployee(employeePeriod.PeriodId, employeePeriod.EmployeeIds, null);
                });
            }
        }

        private void GetAndSendEmailToAllRelatedEmployee(int periodId, string employeeIds, int? processId)
        {
            PeriodModel period = GetPeriodInformation(periodId);
            IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds, processId);

            autoMailService.SendStartMail(period, employeeSettings);
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
