namespace NexGoal.Services.Extensions
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceDIExtensions
    {
        public static void ResolveServiceDI(this IServiceCollection services)
        {
            services.AddScoped<ICryptoService, CryptoService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IAvatarService, AvatarService>();
            services.AddScoped<IFunctionService, FunctionService>();
            services.AddScoped<IEOQService, EOQService>();
            services.AddScoped<IEOYService, EOYService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDisciplineService, DisciplineService>();
            services.AddScoped<IFlowTemplateService, FlowTemplateService>();
            services.AddScoped<IPeriodService, PeriodService>();
            services.AddScoped<IGroupFlowSettingService, GroupFlowSettingService>();
            services.AddScoped<IEmployeeFlowSettingService, EmployeeFlowSettingService>();
            services.AddScoped<IPerformanceCheckInService, PerformanceCheckInService>();
            services.AddScoped<IPARecordService, PARecordService>();
            services.AddScoped<IEmployeeNominationService, EmployeeNominationService>();
            services.AddScoped<IDashboardService, DashboardService>();
        }
    }
}