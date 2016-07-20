namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Criteria.EPAHistory;
    using NexGoal.Model.EPAHistory;
    using NexGoal.Model.RoleManagement;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using NexGoal.Web;
    using NexGoal.Web.ApiAuthorizeAttribute;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    public class EPAHistoryController : BaseController
    {
        private readonly IEPAHistoryService _epaHistoryService;
        private readonly IRoleService roleService;

        public EPAHistoryController()
            : this(null, null)
        {
        }

        public EPAHistoryController(IEPAHistoryService epaHistoryService, IRoleService roleService)
        {
            _epaHistoryService = epaHistoryService ?? new EPAHistoryService();
            this.roleService = roleService ?? new RoleService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAHistory, Policy = Policy.Read)]
        public IList<EPAHistoryModel> GetEPAHistory(int? periodId = null, int? employeeId = null)
        {
            var criteria = new GetEPAHistoryCriteria
            {
                PeriodId = periodId,
                EvaluatorId = SessionManager.CurrentUserId,
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
                EvaluatorId = SessionManager.CurrentUserId,
                EmployeeId = employeeId
            };

            IList<EPAHistoryModel> epas = _epaHistoryService.GetMonthlyEPAHistory(criteria);
            return epas;
        }

        [HttpGet]
        public bool HasOnlyEmployeeRole()
        {
            IEnumerable<SystemRoleModel> roles = this.roleService.GetRolesByEmployeeId(SessionManager.CurrentUserId);
            bool onlyEmployeeRole = roles.All(x => x.RoleId == (int)SystemRole.Employee);
            return onlyEmployeeRole;
        }
    }
}