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
            services.AddScoped<INominationPeriodService, NominationPeriodService>();
            services.AddScoped<IEOQService, EOQService>();
            services.AddScoped<IEOYService, EOYService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IDisciplineService, DisciplineService>();
            services.AddScoped<IPeriodService, PeriodService>();
            services.AddScoped<IGroupFlowSettingService, GroupFlowSettingService>();
            services.AddScoped<IEmployeeFlowSettingService, EmployeeFlowSettingService>();
            services.AddScoped<IPerformanceRecordMailService, PerformanceRecordMailService>();
            services.AddScoped<IPerformanceRecordService, PerformanceRecordService>();
            services.AddScoped<IPARecordService, PARecordService>();
            services.AddScoped<IEmployeeNominationService, EmployeeNominationService>();
            services.AddScoped<IRedmineService, RedmineService>();
            services.AddScoped<IPAFocusService, PAFocusService>();
            services.AddScoped<IExportPAFocusService, ExportPAFocusService>();
            services.AddScoped<INominationReportService, NominationReportService>();
        }
    }
}