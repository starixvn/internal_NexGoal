namespace NexGoal.Core.Middlewares
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IAuthenticationService authenticationService;

        public AuthenticationMiddleware(
            RequestDelegate next,
            IAuthenticationService authenticationService)
        {
            this.next = next;
            this.authenticationService = authenticationService;
        }

        public async Task Invoke(HttpContext context)
        {
            var currentSession = new SessionService(context.Session);

            if (currentSession.EmployeeId <= 0)
            {
                var domainUsername = context.User.Identity.Name;
                EmployeeSession employee = await authenticationService.GetEmployeeByDomainUsernameAsync(domainUsername).ConfigureAwait(false);

                if (employee != null)
                {
                    currentSession.EmployeeId = employee.EmployeeId;
                    currentSession.NickName = employee.NickName;
                    currentSession.FullName = employee.FullName;
                    currentSession.Functions = await authenticationService.GetFunctionByEmployeeIdAsync(employee.EmployeeId).ConfigureAwait(false);
                    currentSession.MenuItems = await authenticationService.GetMenuByEmployeeIdAsync(employee.EmployeeId).ConfigureAwait(false);
                }
            }

            await next(context).ConfigureAwait(false);
        }
    }
}