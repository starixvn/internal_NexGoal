namespace NexGoal.Core.Extensions
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
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
            services.AddTransient<IBaseMailService, BaseMailService>();
            services.AddScoped<ICryptoService, CryptoService>();
            services.AddTransient<ILogger, Logger>();
        }
    }
}