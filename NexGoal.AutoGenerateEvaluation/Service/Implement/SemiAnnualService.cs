using NexGoal.Core.Model;
using NexGoal.Core.Services;
using NexGoal.Criteria.Period.GenerationEvaluation;
using NexGoal.Model.Period;
using NexGoal.Services.Implement;
using NexGoal.Services.Interfaces;
using System.Collections.Generic;

namespace NexGoal.AutoGenerateEvaluation
{
    public class SemiAnnualService : BaseService, ISemiAnnualService
    {
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;

        public SemiAnnualService()
            : this(null, null)
        {
        }

        public SemiAnnualService(
            IPeriodService periodService,
            IAutoMailService autoMailService)
        {
            this.periodService = periodService ?? new PeriodService();
            this.autoMailService = autoMailService ?? new AutoMailService();
        }

        public void GeneratedEPA()
        {
            IEnumerable<int> periods = GetPeriodToGenerateEPA();
            foreach (int periodId in periods)
            {
                AddEmployeeToPeriod(periodId);
                GenerateEPA(periodId);
                SendEmailToAllRelatedEmployee(periodId);
            }
        }

        private IEnumerable<int> GetPeriodToGenerateEPA()
        {
            IEnumerable<int> startedPeriods = new List<int>();
            var criteria = new GetSemiAnnualPeriodStartedCriteria();
            if (criteria != null && criteria.IsValid())
            {
                startedPeriods = GetData<int>(criteria);
            }

            return startedPeriods;
        }

        private void GenerateEPA(int periodId)
        {
            var criteria = new AutoGenerateEvaluationCriteria
            {
                PeriodId = periodId
            };
            periodService.GenerateEvaluation(criteria);
        }

        private CommandResult AddEmployeeToPeriod(int periodId)
        {
            var criteria = new InsertEmployeeToSemiAnnualPeriodCriteria { PeriodId = periodId };
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        private void SendEmailToAllRelatedEmployee(int periodId)
        {
            if (AppSetting.SendMailAllWhenPeriodStarted)
            {
                PeriodModel period = periodService.GetPeriodById(periodId);
                IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId);

                autoMailService.SendStartMail(period, employeeSettings);
            }
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId)
        {
            var criteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId
            };

            IEnumerable<PeriodFlowModel> employeeSettings = periodService.GetEmployeeSettingByPeriodId(criteria);

            return employeeSettings;
        }
    }
}
