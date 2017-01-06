namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Criteria.EPAHistory;
    using NexGoal.Model.EPAHistory;
    using NexGoal.Model.RoleManagement;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
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

        [ApiAuthorize(FunctionKey = FunctionKey.EPAHistory, Policy = Policy.Read)]
        [HttpGet]
        public IList<EPAHistoryModel> GetEPAHistory(int? periodId = null, int? employeeId = null)
        {
            var criteria = new GetEPAHistoryCriteria
            {
                PeriodId = periodId,
                EvaluatorId = SessionService.EmployeeId,
                EmployeeId = employeeId
            };

            IList<EPAHistoryModel> epas = _epaHistoryService.GetEPAHistory(criteria);
            return epas;
        }

        [HttpGet]
        public bool HasOnlyEmployeeRole()
        {
            IEnumerable<SystemRoleModel> roles = this.roleService.GetRolesByEmployeeId(SessionService.EmployeeId);
            bool onlyEmployeeRole = roles.All(x => x.RoleId == (int)SystemRole.Employee);
            return onlyEmployeeRole;
        }
    }
}