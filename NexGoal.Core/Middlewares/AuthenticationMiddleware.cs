namespace NexGoal.Core.Middlewares
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using NexGoal.Core.Criteria;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IBaseService baseService;

        public AuthenticationMiddleware(
            RequestDelegate next,
            IBaseService baseService)
        {
            this.next = next;
            this.baseService = baseService;
        }

        public async Task Invoke(HttpContext context)
        {
            var currentSession = new SessionService(context.Session);

            if (currentSession.EmployeeId <= 0)
            {
                var domainUsername = context.User.Identity.Name;
                EmployeeSession employee = GetEmployeeByDomainUsername(domainUsername);

                if (employee != null)
                {
                    currentSession.EmployeeId = employee.EmployeeId;
                    currentSession.NickName = employee.NickName;
                    currentSession.FullName = employee.FullName;
                    currentSession.Functions = GetFunctionByEmployeeId(employee.EmployeeId);
                    currentSession.MenuItems = GetMenuByEmployeeId(employee.EmployeeId);
                }
            }

            await next(context).ConfigureAwait(false);
        }

        private EmployeeSession GetEmployeeByDomainUsername(string domainUsername)
        {
            var criteria = new GetEmployeeByDomainUsernameCriteria
            {
                DomainUsername = domainUsername
            };

            return baseService.GetDataEntityAsync<EmployeeSession>(criteria).Result;
        }

        private IEnumerable<FunctionSession> GetFunctionByEmployeeId(int employeeId)
        {
            var criteria = new GetFunctionByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            return baseService.GetDataAsync<FunctionSession>(criteria).Result;
        }

        private IEnumerable<MenuItemSession> GetMenuByEmployeeId(int employeeId)
        {
            var criteria = new GetMenuByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            return baseService.GetDataAsync<MenuItemSession>(criteria).Result;
        }
    }
}