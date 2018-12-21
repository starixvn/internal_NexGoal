namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Function;

    public interface IFunctionService
    {
        Task<IEnumerable<FunctionModel>> GetFunctionByRoleIdAsync(int roleId);

        Task<IEnumerable<FunctionModel>> GetFunctionsAsync(bool? isActive = true);

        Task<CommandResult> AssignFunctionToRoleAsync(int actorId, int roleId, IEnumerable<RolePermissionModel> permissions);
    }
}