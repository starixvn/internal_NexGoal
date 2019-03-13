namespace NexGoal.Web.Core.Middlewares
{
    using System.Net;
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

        public async Task InvokeAsync(HttpContext context)
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

                    await WriteLogAsync(domainUsername, context).ConfigureAwait(false);
                }
            }

            await next(context).ConfigureAwait(false);
        }

        private async Task WriteLogAsync(string username, HttpContext context)
        {
            var logInfo = new LogLogin
            {
                Username = username,
                IP = context.Connection?.RemoteIpAddress?.ToString(),
                Platform = context.Request?.Headers["User-Agent"].ToString()
            };
            logInfo.MachineName = GetMachineName(logInfo.IP);

            await authService.WriteLogLoginAsync(logInfo).ConfigureAwait(false);
        }

        private static string GetMachineName(string ipAdress)
        {
            var machineName = string.Empty;
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAdress);
                machineName = hostEntry.HostName;
            }
            catch
            {
                // log here
            }

            return machineName;
        }
    }
}