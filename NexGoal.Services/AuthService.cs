namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Auth;
    using NexGoal.Models.Auth;
    using NexGoal.Models.Common;

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

            return await baseService.GetDataEntityAsync<EmployeeSession>(criteria);
        }

        public async Task<IEnumerable<FunctionSession>> GetFunctionByEmployeeIdAsync(int employeeId)
        {
            var criteria = new GetFunctionByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<FunctionSession>(criteria);
        }

        public async Task<IEnumerable<MenuItemSession>> GetMenuByEmployeeIdAsync(int employeeId)
        {
            var criteria = new GetMenuByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            IEnumerable<MenuItemSession> menuItems = await baseService.GetDataAsync<MenuItemSession>(criteria);
            IEnumerable<MenuItemSession> menuGroups = menuItems.Where(x => x.ParentId == null);
            foreach (MenuItemSession menuGroup in menuGroups)
            {
                menuGroup.Childs = menuItems.Where(x => x.ParentId == menuGroup.MenuId);
            }

            return menuGroups;
        }

        public async Task<ValueTuple<Employee, bool, int>> GetEmployeeInfoAsync(int employeeId)
        {
            var criteria = new GetEmployeeInfoCriteria
            {
                EmployeeId = employeeId
            };

            var info = await baseService.GetDataEntityAsync<Employee>(criteria);

            return new (info, criteria.IsHasMember, criteria.UnreadNotification);
        }

        public async Task WriteLogLoginAsync(LogLogin logInfo)
        {
            var criteria = new WriteLogLoginCriteria
            {
                Username = logInfo.Username,
                IP = logInfo.IP,
                MachineName = logInfo.MachineName,
                Platform = logInfo.Platform
            };

            await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}