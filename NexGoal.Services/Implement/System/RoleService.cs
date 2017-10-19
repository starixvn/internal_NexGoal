namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.RoleManagement;
    using NexGoal.Model.RoleManagement;
    using NexGoal.Services.Interfaces;

    public class RoleService : BaseService, IRoleService
    {
        public IEnumerable<SystemRoleModel> GetRolesByEmployeeId(int employeeId)
        {
            var criteria = new GetRolesByEmployeeIdCriteria { EmployeeId = employeeId };
            IEnumerable<SystemRoleModel> roles = new List<SystemRoleModel>();
            if (criteria.IsValid())
            {
                roles = GetData<SystemRoleModel>(criteria);
            }

            return roles;
        }

        public IEnumerable<RoleFunctionModel> GetFunctionByRoleId(GetFunctionByRoleIdCriteria criteria)
        {
            IEnumerable<RoleFunctionModel> functions = new List<RoleFunctionModel>();

            if (criteria != null && criteria.IsValid())
            {
                IEnumerable<RoleFunctionModel> rawFunctions = GetData<RoleFunctionModel>(criteria);
                functions = GroupFunction(rawFunctions);
            }

            return functions;
        }

        public CommandResult AssignFunctionToRole(AssignFunctionToRoleCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        private static IEnumerable<RoleFunctionModel> GroupFunction(IEnumerable<RoleFunctionModel> rawFunctions)
        {
            IEnumerable<RoleFunctionModel> functions = rawFunctions.Where(func => func.ParentFunctionId == null);

            foreach (RoleFunctionModel function in functions)
            {
                function.ChildFunctions = rawFunctions.Where(x => x.ParentFunctionId == function.FunctionId).ToList();
            }

            return functions;
        }
    }
}