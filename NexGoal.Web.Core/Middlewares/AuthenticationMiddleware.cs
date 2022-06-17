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
        private readonly LdapSettings ldapSetting;

        public AuthenticationMiddleware(
            RequestDelegate next,
            IAuthService authService,
            LdapSettings ldapSetting)
        {
            this.next = next;
            this.authService = authService;
            this.ldapSetting = ldapSetting;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var currentSession = new SessionService(context.Session);

            if (currentSession.EmployeeId <= 0)
            {
                var domainUsername = string.Equals(ldapSetting.CompanyName, "Luminet")
                    ? ldapSetting.Domain + context.User.Identity.Name
                    : context.User.Identity.Name;
                EmployeeSession employee = await authService.GetEmployeeByDomainUsernameAsync(domainUsername);

                if (employee != null)
                {
                    currentSession.EmployeeId = employee.EmployeeId;
                    currentSession.NickName = employee.NickName;
                    currentSession.FullName = employee.FullName;
                    currentSession.IsInProbation = employee.IsInProbation;
                    currentSession.Functions = await authService.GetFunctionByEmployeeIdAsync(employee.EmployeeId);
                    currentSession.MenuItems = await authService.GetMenuByEmployeeIdAsync(employee.EmployeeId);

                    await WriteLogAsync(domainUsername, context);
                }
            }

            await next(context);
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

            await authService.WriteLogLoginAsync(logInfo);
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