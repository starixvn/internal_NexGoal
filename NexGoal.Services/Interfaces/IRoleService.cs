namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.RoleManagement;
    using NexGoal.Model.RoleManagement;

    public interface IRoleService
    {
        IEnumerable<RoleFunctionModel> GetFunctionByRoleId(GetFunctionByRoleIdCriteria criteria);

        CommandResult AssignFunctionToRole(AssignFunctionToRoleCriteria criteria);
    }
}