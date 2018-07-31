namespace NexGoal.Services.Extensions
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceDIExtensions
    {
        public static void ResolveServiceDI(this IServiceCollection services)
        {
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<IFunctionService, FunctionService>();
            services.AddTransient<IEOQService, EOQService>();
            services.AddTransient<IEOYService, EOYService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDisciplineService, DisciplineService>();
            services.AddScoped<IFlowTemplateService, FlowTemplateService>();
        }
    }
}