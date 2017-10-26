namespace NexGoal.BackgroundService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Model;
    using Fanex.Logging;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Period.GenerationEvaluation;
    using NexGoal.Model.Period;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;

    public class ProbationService : BaseService, IProbationService
    {
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;
        private readonly IEvaluationRepository evaluationRepository;

        public ProbationService()
            : this(null, null, null)
        {
        }

        public ProbationService(
            IEvaluationRepository evaluationRepository,
            IPeriodService periodService,
            IAutoMailService autoMailService)
        {
            this.evaluationRepository = evaluationRepository ?? new EvaluationRepository();
            this.periodService = periodService ?? new PeriodService();
            this.autoMailService = autoMailService ?? new AutoMailService();
        }

        public void GeneratedEPA()
        {
            AutoEmployeePeriodModel employeePeriod = GetEmployeeProbationEvaluation();
            if (!string.IsNullOrWhiteSpace(employeePeriod.EmployeeIds) && employeePeriod.PeriodId > 0)
            {
                AddEmployeeToProbationPeriod(employeePeriod);
                GenerateEPAProbation(employeePeriod);
                SendEmailToAllRelatedEmployee(employeePeriod.PeriodId, employeePeriod.EmployeeIds);
            }
        }

        private AutoEmployeePeriodModel GetEmployeeProbationEvaluation()
        {
            var employeePeriod = new AutoEmployeePeriodModel();
            var criteria = new GetEmployeeProbationEvaluationCriteria();

            if (criteria.IsValid())
            {
                employeePeriod = evaluationRepository.GetEmployeeProbationEvaluation(criteria);
            }

            return employeePeriod;
        }

        private void AddEmployeeToProbationPeriod(AutoEmployeePeriodModel employeePeriod)
        {
            var criteria = new InsertEmployeeToProbationPeriodCriteria
            {
                PeriodId = employeePeriod.PeriodId,
                EmployeeIds = employeePeriod.EmployeeIds
            };

            if (criteria.IsValid())
            {
                CommandResult commandResult = Repository.ExecuteNonQuery(criteria);

                if (!commandResult.IsSuccessful)
                {
                    LogService.Error(commandResult.Message);
                }
            }
        }

        private void GenerateEPAProbation(AutoEmployeePeriodModel employeePeriod)
        {
            var criteria = new AutoGenerateEvaluationCriteria
            {
                PeriodId = employeePeriod.PeriodId,
                EmployeeIds = employeePeriod.EmployeeIds
            };
            periodService.GenerateEvaluation(criteria);
        }

        private void SendEmailToAllRelatedEmployee(int periodId, string employeeIds)
        {
            if (AppSetting.SendMailAllWhenPeriodStarted)
            {
                Task.Factory.StartNew(() =>
                {
                    PeriodModel period = periodService.GetPeriodById(periodId);
                    IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds);

                    autoMailService.SendStartMail(period, employeeSettings);
                });
            }
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId, string employeeIds)
        {
            var criteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds
            };
            IEnumerable<PeriodFlowModel> employeeSettings = periodService.GetEmployeeSettingByPeriodId(criteria);

            return employeeSettings;
        }
    }
}