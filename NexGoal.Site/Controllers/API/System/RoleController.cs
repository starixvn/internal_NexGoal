namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.RoleManagement;
    using NexGoal.Model.RoleManagement;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    public class RoleController : BaseController
    {
        private IRoleService _roleService;

        public RoleController()
            : this(null)
        {
        }

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService ?? new RoleService();
        }

        [ApiAuthorize(FunctionKey = FunctionKey.RoleManagement, Policy = Policy.Read)]
        public IEnumerable<RoleFunctionModel> GetRoleFunctionByRoleId(int roleId)
        {
            var criteria = new GetFunctionByRoleIdCriteria
            {
                RoleId = roleId
            };

            IEnumerable<RoleFunctionModel> functions = _roleService.GetFunctionByRoleId(criteria);
            return functions;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.RoleManagement, Policy = Policy.Host)]
        [HttpPut]
        public CommandResult AssignFunctionToRole(AssignFunctionToRoleCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = _roleService.AssignFunctionToRole(criteria);
            return commandResult;
        }
    }
}