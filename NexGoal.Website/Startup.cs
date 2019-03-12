namespace NexGoal.Website
{
    using System;
    using System.Collections.Generic;
    using System.IO.Compression;
    using System.Linq;
    using DinkToPdf;
    using DinkToPdf.Contracts;
    using Fanex.Data;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using Hangfire;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.AspNetCore.Server.IISIntegration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Extensions;
    using NexGoal.Core;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Services.Extensions;
    using NexGoal.Web.Core.Extensions;
    using NexGoal.Web.Core.Middlewares;
    using Sentry;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(options => Configuration.GetSection("AppSettings").Bind(options));
            services.Configure<MailConfigs>(options => Configuration.GetSection("MailConfigs").Bind(options));
            services.Configure<ChatbotConfigs>(options => Configuration.GetSection("ChatbotConfigs").Bind(options));
            //// string encryptedPassword = Cryptography.EncryptPassword("@357$abc^FFZ%");
            Dictionary<string, string> connections = Configuration
                .GetSection("ConnectionStrings")
                .GetChildren()
                .ToDictionary(connection => connection.Key, connection => connection.Value, StringComparer.OrdinalIgnoreCase);

            int.TryParse(Configuration.GetSection("ConnectionStringsEncryptionAlgorithm").Value, out int algorithm);

            DbSettingProviderManager
                .StartNewSession()
                .Use(new CustomConnectionStringProvider(connections, algorithm, services.BuildServiceProvider().GetService<IOptionsMonitor<AppSettings>>()))
                .UseDefaultDbSettingProvider("~/App_Data")
                .Run();

            LogManager
            .SetDefaultLogCategory(Configuration.GetSection("LogCategory").Value)
            .Use(new List<ILogEngine>
            {
                new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(Configuration.GetSection("SentryEngineOptions:Dsn").Value),
                    FrontEndDsn = new Dsn(Configuration.GetSection("SentryEngineOptions:FrontEndDsn").Value)
                })
            });

            var hangfireConnectionString = Cryptography.DecryptConnectionString(Configuration.GetSection("HangfireConnectionString").Value);
            services.AddHangfire(config => config.UseSqlServerStorage(hangfireConnectionString));

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });
            services.AddAuthentication(IISDefaults.AuthenticationScheme);

            services.AddMemoryCache();

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
                options.EnableForHttps = true;
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddSession();

            // DI
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.ResolveCoreDI();
            services.ResolveServiceDI();
            services.ResolveWebCoreDI();
            services.ResolveAutoServiceDI();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ErrorHandlingMiddleware>();
                app.UseHsts();
            }

            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();
            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseHangfireServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}