namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Criteria.EmployeeEPA.RatingByKPI;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.EmployeeEPA.RatingByKPI;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.TargetTemplate;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    public class EPAByKPIController : BaseController
    {
        private readonly IEmployeeEPAService _employeeEPAService;

        public EPAByKPIController()
            : this(null)
        {
        }

        public EPAByKPIController(IEmployeeEPAService employeeEPAService)
        {
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
        }

        [ApiAuthorize(FunctionKey.EPAByKPI, Policy.Read)]
        [HttpGet]
        public IEnumerable<EPAPeriodModel> GetPeriodToView()
        {
            var criteria = new GetPeriodToViewCriteria()
            {
                EvaluatorId = SessionService.EmployeeId
            };

            return _employeeEPAService.GetPeriodToView(criteria);
        }

        [ApiAuthorize(FunctionKey.EPAByKPI, Policy.Read)]
        [HttpGet]
        public IEnumerable<TargetTemplateModel> GetTargetTemplateToView(int periodId)
        {
            var criteria = new GetTargetTemplateToViewCriteria()
            {
                EvaluatorId = SessionService.EmployeeId,
                PeriodId = periodId
            };

            return _employeeEPAService.GetTargetTemplateToView(criteria);
        }

        [ApiAuthorize(FunctionKey.EPAByKPI, Policy.Read)]
        [HttpGet]
        public IEnumerable<Employee> GetEvaluatorsByTargetTemplate(
            int periodId,
            int targetTemplateId)
        {
            var criteria = new GetEvaluatorsByTargetTemplateCriteria()
            {
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId
            };

            return _employeeEPAService.GetEvaluatorsByTargetTemplate(criteria);
        }

        [ApiAuthorize(FunctionKey.EPAByKPI, Policy.Read)]
        [HttpGet]
        public IEnumerable<TemporaryKPIModel> GetKPIToView(
            int periodId,
            int targetTemplateId,
            int evaluatorId)
        {
            var criteria = new GetKPIToViewCriteria()
            {
                EvaluatorId = evaluatorId,
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId
            };

            return _employeeEPAService.GetKPIToView(criteria);
        }

        [ApiAuthorize(FunctionKey.EPAByKPI, Policy.Read)]
        [HttpGet]
        public IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(
            int periodId,
            int targetTemplateId,
            int kpiId,
            int evaluatorId)
        {
            var criteria = new GetEmployeeEPAToViewCriteria()
            {
                EvaluatorId = evaluatorId,
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId,
                KPIId = kpiId
            };

            return _employeeEPAService.GetEmployeeEPAToView(criteria);
        }
    }
}