namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Function;
    using NexGoal.Models.Function;

    public class FunctionService : IFunctionService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;

        public FunctionService(
            IBaseService baseService,
            ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
        }

        public async Task<IEnumerable<FunctionModel>> GetFunctionByRoleIdAsync(int roleId)
        {
            var criteria = new GetFunctionByRoleIdCriteria
            {
                RoleId = roleId
            };

            return await baseService.GetDataAsync<FunctionModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<FunctionModel>> GetTreeFunctionsAsync(bool? isActive = true)
        {
            IEnumerable<FunctionModel> functions = await GetFunctions(isActive).ConfigureAwait(false);
            IEnumerable<FunctionModel> parentFunctions = new List<FunctionModel>();

            if (functions != null && functions.Any())
            {
                parentFunctions = functions.Where(x => !x.ParentId.HasValue);

                foreach (var function in parentFunctions)
                {
                    function.Level = 0;
                    GetChildFunctions(function, functions);
                }
            }

            return parentFunctions;
        }

        public async Task<CommandResult> AssignFunctionToRoleAsync(int roleId, IEnumerable<RolePermissionModel> permissions)
        {
            var criteria = new AssignFunctionToRoleCriteria
            {
                ModifiedBy = sessionService.EmployeeId,
                RoleId = roleId,
                Value = permissions.ToXML().ToString()
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        private async Task<IEnumerable<FunctionModel>> GetFunctions(bool? isActive)
        {
            var criteria = new GetAllFunctionCriteria
            {
                IsActive = isActive
            };

            return await baseService.GetDataAsync<FunctionModel>(criteria).ConfigureAwait(false);
        }

        private void GetChildFunctions(FunctionModel function, IEnumerable<FunctionModel> functions)
        {
            IEnumerable<FunctionModel> functionItems = functions.Where(x => x.ParentId == function.FunctionId);

            if (functionItems.Any())
            {
                foreach (FunctionModel item in functionItems)
                {
                    item.Level = function.Level + 1;
                    function.Childs.Add(item);
                    GetChildFunctions(item, functions);
                }
            }
        }
    }
}