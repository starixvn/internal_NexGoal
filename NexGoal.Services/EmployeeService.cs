namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Employee;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeRole;

    public class EmployeeService : IEmployeeService
    {
        private readonly IBaseService baseService;

        public EmployeeService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(GetEmployeeParam param)
        {
            var criteria = new GetEmployeesCriteria
            {
                Keyword = param.Keyword,
                GroupId = param.GroupId,
                RoleIds = param.RoleIds,
                IsDisabled = param.IsDisabled
            };

            return await baseService.GetDataAsync<Employee>(criteria).ConfigureAwait(false);
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataEntityAsync<Employee>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<EmployeeRoleModel>> GetRolesByEmployeeIdAsync(int employeeId)
        {
            var criteria = new GetRolesByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<EmployeeRoleModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditEmployeeRolesAsync(EditEmployeeRolesModel model)
        {
            var criteria = new EditEmployeeRolesCriteria
            {
                EmployeeId = model.EmployeeId,
                SelectedRoleIds = model.SelectedRoleIds,
                ModifiedBy = model.ModifiedBy
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<int> GetRoleAccordingToMemberAsync(int actorId, int memberId)
        {
            var criteria = new GetRoleAccordingToMemberCriteria
            {
                ActorId = actorId,
                MemberId = memberId
            };

            return await baseService.GetDataEntityAsync<int>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Employee>> GetMemberAsync(int actorId, string keyword = null)
        {
            var criteria = new GetMemberByActorIdCriteria
            {
                ActorId = actorId,
                Keyword = keyword
            };

            return await baseService.GetDataAsync<Employee>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupBaseModel>> GetGroupAsync(int actorId)
        {
            var criteria = new GetGroupByActorIdCriteria
            {
                ActorId = actorId
            };

            return await baseService.GetDataAsync<GroupBaseModel>(criteria).ConfigureAwait(false);
        }
    }
}