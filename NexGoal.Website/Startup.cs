namespace NexGoal.Website
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Server.IISIntegration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using NexGoal.Core.Middlewares;
    using NexGoal.Core.Services;

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

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });
            services.AddAuthentication(IISDefaults.AuthenticationScheme);

            services.AddMvc();

            services.AddSession();

            // DI
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IBaseService, BaseService>();
            services.AddTransient<ISessionService, SessionService>();
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
                app.UseExceptionHandler("/Error/InternalError");
                app.UseHsts();

                app.UseMiddleware<ErrorHandlingMiddleware>();
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