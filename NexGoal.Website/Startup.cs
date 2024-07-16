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
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NexGoal.Core;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Models.Auth;
    using NexGoal.Services.Extensions;
    using NexGoal.Web.Core.Middlewares;
    using NexGoal.Web.Core.Services;
    using Sentry;
    using StackExchange.Redis;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
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

            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            services.AddSingleton(appSettings);

            var mailConfigs = Configuration.GetSection("MailConfigs").Get<MailConfigs>();
            services.AddSingleton(mailConfigs);

            var chatbotConfigs = Configuration.GetSection("ChatbotConfigs").Get<ChatbotConfigs>();
            services.AddSingleton(chatbotConfigs);

            var ldapSettings = Configuration.GetSection("LDAPSettings").Get<LdapSettings>();
            services.AddSingleton(ldapSettings);

            var accessSettings = Configuration.GetSection("AccessControlSettings").Get<AccessControlSettings>();
            services.AddSingleton(accessSettings);

            var connections = Configuration.GetSection("ConnectionStrings")
                .GetChildren()
                .AsEnumerable()
                .Where(x => !string.IsNullOrWhiteSpace(x.GetValue<string>("Name")))
                .ToDictionary(x => x.GetValue<string>("Name"),
                    y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName")));

            _ = byte.TryParse(Configuration.GetSection("ConnectionStringsEncryptionAlgorithm").Value, out byte algorithm);

            DbSettingProviderManager
                .StartNewSession()
                .Use(new CustomConnectionStringProvider(connections, algorithm, appSettings))
                .WithSqlServer()
                .UseDefaultDbSettingProvider("~/App_Data")
                .Run();

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
                options.EnableForHttps = true;
            });

            var companyName = Configuration.GetSection("CompanyName").Value;

            services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));

            ConfigAuthentication(services, companyName);

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(int.Parse(Configuration["AuthSettings:SessionTimeout"]));
                options.Cookie.IsEssential = true;
                options.Cookie.Name = $"Goal.{companyName}.Session";
            });

            ConfigDataProtection(services);

            services.AddHttpClient();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<ISessionService, SessionService>();

            services.ResolveCoreDI();
            services.ResolveServiceDI();

            services.AddControllersWithViews()
                .AddMvcOptions(o =>
                    o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute())
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseCookiePolicy();
            app.UseRouting();

            app.UseMiddleware<AccessControlMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigAuthentication(IServiceCollection services, string companyName)
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
                options.Cookie = new CookieBuilder
                {
                    Name = $"Goal.{companyName}.Cookie",
                };
            });

            services.AddAuthorization();
        }

        private void ConfigDataProtection(IServiceCollection services)
        {
            var redisConnectionString = Configuration.GetSection("RedisConfig:ConnectionString").Value;
            var appName = Configuration.GetSection("RedisConfig:ApplicationName").Value;

            var redis = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddDataProtection()
                .SetApplicationName(appName)
                .PersistKeysToStackExchangeRedis(redis, $"{appName}:DataProtection:Keys");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = $"{appName}:Session:";
            });
        }
    }
}