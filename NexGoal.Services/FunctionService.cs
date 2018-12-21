namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Function;
    using NexGoal.Models.Function;

    public class FunctionService : IFunctionService
    {
        private readonly IBaseService baseService;

        public FunctionService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<FunctionModel>> GetFunctionByRoleIdAsync(int roleId)
        {
            var criteria = new GetFunctionByRoleIdCriteria
            {
                RoleId = roleId
            };

            return await baseService.GetDataAsync<FunctionModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<FunctionModel>> GetFunctionsAsync(bool? isActive = true)
        {
            var criteria = new GetAllFunctionCriteria
            {
                IsActive = isActive
            };

            return await baseService.GetDataAsync<FunctionModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> AssignFunctionToRoleAsync(int actorId, int roleId, IEnumerable<RolePermissionModel> permissions)
        {
            var criteria = new AssignFunctionToRoleCriteria
            {
                ModifiedBy = actorId,
                RoleId = roleId,
                Value = permissions.ToXML().ToString()
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}