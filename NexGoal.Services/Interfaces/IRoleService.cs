namespace NexGoal.Services.Interfaces
{
    using NexGoal.Core.Model;
    using NexGoal.Criteria.RoleManagement;
    using NexGoal.Model.RoleManagement;
    using System.Collections.Generic;

    public interface IRoleService
    {
        IEnumerable<SystemRoleModel> GetRolesByEmployeeId(int employeeId);

        IEnumerable<RoleFunctionModel> GetFunctionByRoleId(GetFunctionByRoleIdCriteria criteria);

        CommandResult AssignFunctionToRole(AssignFunctionToRoleCriteria criteria);
    }
}