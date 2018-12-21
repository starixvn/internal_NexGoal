namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Auth;
    using NexGoal.Models.Auth;

    public class AuthService : IAuthService
    {
        private readonly IBaseService baseService;

        public AuthService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<EmployeeSession> GetEmployeeByDomainUsernameAsync(string domainUsername)
        {
            var criteria = new GetEmployeeByDomainUsernameCriteria
            {
                DomainUsername = domainUsername
            };

            return await baseService.GetDataEntityAsync<EmployeeSession>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<FunctionSession>> GetFunctionByEmployeeIdAsync(int employeeId)
        {
            var criteria = new GetFunctionByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<FunctionSession>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<MenuItemSession>> GetMenuByEmployeeIdAsync(int employeeId)
        {
            var criteria = new GetMenuByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            IEnumerable<MenuItemSession> menuItems = await baseService.GetDataAsync<MenuItemSession>(criteria).ConfigureAwait(false);
            IEnumerable<MenuItemSession> menuGroups = menuItems.Where(x => x.ParentId == null);
            foreach (MenuItemSession menuGroup in menuGroups)
            {
                menuGroup.Childs = menuItems.Where(x => x.ParentId == menuGroup.MenuId);
            }

            return menuGroups;
        }
    }
}