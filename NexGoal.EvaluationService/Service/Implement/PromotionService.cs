using NexGoal.Core.Services;
using NexGoal.Criteria.Period.GenerationEvaluation;
using NexGoal.Model.Period;
using NexGoal.Services.Implement;
using NexGoal.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexGoal.AutoGenerateEvaluation
{
    public class PromotionService : BaseService, IPromotionService
    {
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;

        public PromotionService()
            : this(null, null)
        {
        }

        public PromotionService(
            IPeriodService periodService,
            IAutoMailService autoMailService)
        {
            this.periodService = periodService ?? new PeriodService();
            this.autoMailService = autoMailService ?? new AutoMailService();
        }

        public void GeneratedEPA()
        {
            IEnumerable<AutoEmployeePeriodModel> employeePeriods = GetEmployeeToGenerateEPA();
            foreach (AutoEmployeePeriodModel period in employeePeriods)
            {
                GenerateEPAPromotion(period);
                SendEmailToAllRelatedEmployee(period.PeriodId, period.EmployeeIds);
            }
        }

        private IEnumerable<AutoEmployeePeriodModel> GetEmployeeToGenerateEPA()
        {
            IEnumerable<AutoEmployeePeriodModel> employeePeriods = new List<AutoEmployeePeriodModel>();
            var criteria = new GetEmployeePromotionEvaluationCriteria();
            if (criteria != null && criteria.IsValid())
            {
                employeePeriods = GetData<AutoEmployeePeriodModel>(criteria);
                employeePeriods = employeePeriods.GroupBy(x => x.PeriodId).Select(x => new AutoEmployeePeriodModel
                {
                    PeriodId = x.Key,
                    EmployeeIds = string.Join(",", x.Select(i => i.EmployeeId.ToString()))
                });
            }

            return employeePeriods;
        }

        private void GenerateEPAPromotion(AutoEmployeePeriodModel period)
        {
            var criteria = new AutoGenerateEvaluationCriteria
            {
                PeriodId = period.PeriodId,
                EmployeeIds = period.EmployeeIds
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
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();

            var getEmployeeSettingCriteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds
            };
            employeeSettings = periodService.GetEmployeeSettingByPeriodId(getEmployeeSettingCriteria);

            return employeeSettings;
        }
    }
}
