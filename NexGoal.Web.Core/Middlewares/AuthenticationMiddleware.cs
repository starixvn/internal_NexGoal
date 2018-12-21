namespace NexGoal.Web.Core.Middlewares
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using NexGoal.Models.Auth;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;

    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IAuthService authService;

        public AuthenticationMiddleware(
            RequestDelegate next,
            IAuthService authService)
        {
            this.next = next;
            this.authService = authService;
        }

        public async Task Invoke(HttpContext context)
        {
            var currentSession = new SessionService(context.Session);

            if (currentSession.EmployeeId <= 0)
            {
                var domainUsername = context.User.Identity.Name;
                EmployeeSession employee = await authService.GetEmployeeByDomainUsernameAsync(domainUsername).ConfigureAwait(false);

                if (employee != null)
                {
                    currentSession.EmployeeId = employee.EmployeeId;
                    currentSession.NickName = employee.NickName;
                    currentSession.FullName = employee.FullName;
                    currentSession.Functions = await authService.GetFunctionByEmployeeIdAsync(employee.EmployeeId).ConfigureAwait(false);
                    currentSession.MenuItems = await authService.GetMenuByEmployeeIdAsync(employee.EmployeeId).ConfigureAwait(false);
                }
            }

            await next(context).ConfigureAwait(false);
        }
    }
}