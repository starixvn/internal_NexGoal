namespace NexGoal.AutoService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using NexGoal.AutoService.Models;
    using NexGoal.AutoService.Repositories;
    using NexGoal.AutoService.Services;
    using Serilog;

    public static class Program
    {
        public static IConfiguration Configuration { get; set; }

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            Configuration = builder.Build();

            Dictionary<string, string> connections = Configuration
                .GetSection("ConnectionStrings")
                .GetChildren()
                .ToDictionary(connection => connection.Key, connection => connection.Value, StringComparer.OrdinalIgnoreCase);

            DbSettingProviderManager
                .StartNewSession()
                .UseConnectionStrings(connections)
                .UseDefaultDbSettingProvider("~/App_Data")
                .Run();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var appService = serviceProvider.GetService<IAppService>();
            appService.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.ConfigurationSection(Configuration.GetSection("Serilog"))
                .CreateLogger();

            services.AddOptions();
            services.Configure<AppSettings>(options => Configuration.GetSection("AppSettings").Bind(options));

            services.AddTransient<IDynamicRepository, DynamicRepository>();
            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddTransient<IBaseMailService, BaseMailService>();
            services.AddTransient<IBaseService, BaseService>();
            services.AddTransient<IPerformanceRecordService, PerformanceRecordService>();
            services.AddTransient<IPeriodService, PeriodService>();

            services.AddTransient<IAppService, AppService>();
        }
    }
}