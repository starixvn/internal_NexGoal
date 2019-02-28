namespace NexGoal.AutoService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Services;
    using NexGoal.Core;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using Sentry;

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

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            int.TryParse(Configuration.GetSection("ConnectionStringsEncryptionAlgorithm").Value, out int algorithm);

            DbSettingProviderManager
                .StartNewSession()
                .Use(new CustomConnectionStringProvider(connections, algorithm, serviceProvider.GetService<IOptionsMonitor<AppSettings>>()))
                .UseDefaultDbSettingProvider("~/App_Data")
                .Run();

            var appService = serviceProvider.GetService<IAppService>();
            appService.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            LogManager
            .SetDefaultLogCategory(Configuration.GetSection("LogCategory").Value)
            .Use(new List<ILogEngine>
            {
                new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(Configuration.GetSection("SentryEngineOptions:Dsn").Value)
                })
            });

            services.AddOptions();
            services.Configure<AppSettings>(options => Configuration.GetSection("AppSettings").Bind(options));
            services.Configure<MailConfigs>(options => Configuration.GetSection("MailConfigs").Bind(options));

            services.ResolveCoreDI();
            services.AddTransient<IPeriodService, PeriodService>();
            services.AddTransient<IPAService, PAService>();
            services.AddTransient<IPAFocusService, PAFocusService>();
            services.AddTransient<INominationService, NominationService>();
            services.AddTransient<IProbationService, ProbationService>();

            services.AddTransient<IAppService, AppService>();
        }
    }
}