namespace NexGoal.Web.Core.Extensions
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using NexGoal.Web.Core.Services;

    public static class WebCoreDIExtensions
    {
        public static void ResolveWebCoreDI(this IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<ISessionService, SessionService>();
        }
    }
}