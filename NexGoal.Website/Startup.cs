namespace NexGoal.Website
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Server.IISIntegration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
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

            DbSettingProviderManager
                .StartNewSession()
                .UseConnectionStrings(connections)
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

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });
            services.AddAuthentication(IISDefaults.AuthenticationScheme);

            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            services.AddSession();

            // DI
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.ResolveCoreDI();
            services.ResolveServiceDI();
            services.ResolveWebCoreDI();
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();
            app.UseMiddleware<AuthenticationMiddleware>();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}