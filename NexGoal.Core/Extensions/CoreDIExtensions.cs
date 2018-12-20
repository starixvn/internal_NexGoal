namespace NexGoal.Core.Extensions
{
    using Fanex.Data.Repository;
    using Microsoft.Extensions.DependencyInjection;
    using NexGoal.Core.Repositories;
    using NexGoal.Core.Services;

    public static class CoreDIExtensions
    {
        public static void ResolveCoreDI(this IServiceCollection services)
        {
            services.AddTransient<IDynamicRepository, DynamicRepository>();
            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddTransient<IBaseService, BaseService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<ISessionService, SessionService>();
            services.AddTransient<IBaseMailService, BaseMailService>();
        }
    }
}