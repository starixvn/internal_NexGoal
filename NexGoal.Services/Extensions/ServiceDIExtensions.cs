namespace NexGoal.Services.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using NexGoal.Repositories.Extensions;
    using NexGoal.Services.Interfaces;

    public static class ServiceDIExtensions
    {
        public static void ResolveServiceDI(this IServiceCollection services)
        {
            services.ResolveRepositoryDI();
            services.AddTransient<IAuthService, AuthService>();
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
            services.AddScoped<INotificationPerformanceRecordService, NotificationPerformanceRecordService>();
            services.AddScoped<IPerformanceRecordService, PerformanceRecordService>();
            services.AddScoped<IPARecordService, PARecordService>();
            services.AddScoped<IEmployeeNominationService, EmployeeNominationService>();
            services.AddScoped<IRedmineService, RedmineService>();
            services.AddScoped<IPAFocusService, PAFocusService>();
            services.AddScoped<IExportPAFocusService, ExportPAFocusService>();
            services.AddScoped<IPAReportService, PAReportService>();
            services.AddScoped<IExportPAReportService, ExportPAReportService>();
            services.AddScoped<INominationReportService, NominationReportService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPMSettingService, PMSettingService>();
            services.AddScoped<IMasterReportService, MasterReportService>();
            services.AddScoped<IChatbotService, ChatbotService>();
        }
    }
}