namespace NexGoal.Web.Core.Extensions
{
    using Fanex.Logging;
    using Microsoft.Extensions.DependencyInjection;
    using NexGoal.Web.Core.Services;

    public static class WebCoreDIExtensions
    {
        public static void ResolveWebCoreDI(this IServiceCollection services)
        {
            services.AddTransient<ILogger, Logger>();
            services.AddTransient<ISessionService, SessionService>();
        }
    }
}