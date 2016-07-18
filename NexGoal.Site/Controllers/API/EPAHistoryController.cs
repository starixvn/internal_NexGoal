namespace NexGoal.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Criteria.EPAHistory;
    using NexGoal.Model.EPAHistory;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using NexGoal.Web.ApiAuthorizeAttribute;

    public class EPAHistoryController : BaseController
    {
        private readonly IEPAHistoryService _epaHistoryService;

        public EPAHistoryController()
            : this(null)
        {
        }

        public EPAHistoryController(IEPAHistoryService epaHistoryService)
        {
            _epaHistoryService = epaHistoryService ?? new EPAHistoryService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAHistory, Policy = Policy.Read)]
        public IList<EPAHistoryModel> GetEPAHistory(int? periodId = null, int? employeeId = null)
        {
            var criteria = new GetEPAHistoryCriteria
            {
                PeriodId = periodId,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                EmployeeId = employeeId
            };

            IList<EPAHistoryModel> epas = _epaHistoryService.GetEPAHistory(criteria);
            return epas;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyEPAHistory, Policy = Policy.Read)]
        public IList<EPAHistoryModel> GetMonthlyEPAHistory(int? periodId = null, int? employeeId = null)
        {
            var criteria = new GetMonthlyEPAHistoryCriteria
            {
                PeriodId = periodId,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                EmployeeId = employeeId
            };

            IList<EPAHistoryModel> epas = _epaHistoryService.GetMonthlyEPAHistory(criteria);
            return epas;
        }

        [HttpGet]
        public bool HasOnlyEmployeeRole()
        {
            bool onlyEmployeeRole = CurrentUser.Roles.All(x => x.RoleId == (int)SystemRole.Employee);
            return onlyEmployeeRole;
        }
    }
}