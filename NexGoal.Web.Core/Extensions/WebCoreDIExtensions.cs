namespace NexGoal.Web.Core.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using NexGoal.Web.Core.Services;

    public static class WebCoreDIExtensions
    {
        public static void ResolveWebCoreDI(this IServiceCollection services)
        {
            services.AddTransient<ISessionService, SessionService>();
        }
    }
}