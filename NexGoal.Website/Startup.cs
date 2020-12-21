namespace NexGoal.Website
{
    using System;
    using System.Collections.Generic;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading.Tasks;
    using DinkToPdf;
    using DinkToPdf.Contracts;
    using Fanex.Data;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using Hangfire;
    using Microsoft.AspNetCore.Authentication.Cookies;
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
    using NexGoal.Models.Auth;
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
                options.AutomaticAuthentication = true;
            });

            var companyName = Configuration.GetSection("CompanyName").Value;
            ConfigureAuthentication(services, companyName);

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
                options.EnableForHttps = true;
            });

            services.AddMemoryCache();

            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            services.AddSession();

            var settings = Configuration.GetSection("LDAPSettings");
            services.Configure<LdapSettings>(options => settings.Bind(options));
            var ldapSettings = settings.Get<LdapSettings>();
            ldapSettings = ldapSettings ?? new LdapSettings();
            ldapSettings.CompanyName = companyName;

            // DI
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddSingleton(ldapSettings);
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
            app.UseSession();
            app.UseAuthentication();
            app.UseMiddleware<AuthenticationMiddleware>();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureAuthentication(IServiceCollection services, string companyName)
        {
            if (string.Equals(companyName, "Luminet"))
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddCookie(options =>
                {
                    options.LoginPath = Configuration.GetSection("AuthSettings:LoginPath").Value;
                    options.LogoutPath = Configuration.GetSection("AuthSettings:LogoutPath").Value;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(Configuration["AuthSettings:SessionTimeout"]));
                    options.SlidingExpiration = true;
                    options.Events.OnSigningOut = context =>
                    {
                        context.Response.Redirect(options.LoginPath);
                        return Task.CompletedTask;
                    };
                });
            }
            else
            {
                services.AddAuthentication(IISDefaults.AuthenticationScheme);
            }
        }
    }
}