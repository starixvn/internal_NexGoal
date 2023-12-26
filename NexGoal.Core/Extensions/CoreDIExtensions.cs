namespace NexGoal.Core.Extensions
{
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Microsoft.Extensions.DependencyInjection;
    using NexGoal.Core.Repositories;
    using NexGoal.Core.Services;

    public static class CoreDIExtensions
    {
        public static void ResolveCoreDI(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<IBaseRepository, BaseRepository>();
            services.AddSingleton<IChatbotService, ChatbotService>();
            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<IBaseMailService, BaseMailService>();
            services.AddSingleton<ICryptoService, CryptoService>();
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<ICacheService, CacheService>();
        }
    }
}