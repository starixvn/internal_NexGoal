namespace NexGoal.Services.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using NexGoal.Repositories.Extensions;
    using NexGoal.Services;

    public static class ServiceDIExtensions
    {
        public static void ResolveServiceDI(this IServiceCollection services)
        {
            services.ResolveRepositoryDI();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ICommonService, CommonService>();
            services.AddSingleton<IFunctionService, FunctionService>();
            services.AddSingleton<INominationPeriodService, NominationPeriodService>();
            services.AddSingleton<IEOQService, EOQService>();
            services.AddSingleton<IEOYService, EOYService>();
            services.AddSingleton<IEmployeeService, EmployeeService>();
            services.AddSingleton<IDisciplineService, DisciplineService>();
            services.AddSingleton<IPeriodService, PeriodService>();
            services.AddSingleton<IGroupFlowSettingService, GroupFlowSettingService>();
            services.AddSingleton<IEmployeeFlowSettingService, EmployeeFlowSettingService>();
            services.AddSingleton<INotificationPerformanceRecordService, NotificationPerformanceRecordService>();
            services.AddSingleton<IPerformanceRecordService, PerformanceRecordService>();
            services.AddSingleton<IPARecordService, PARecordService>();
            services.AddSingleton<IEmployeeNominationService, EmployeeNominationService>();
            services.AddSingleton<IRedmineService, RedmineService>();
            services.AddSingleton<IPAFocusService, PAFocusService>();
            services.AddSingleton<IExportPAFocusService, ExportPAFocusService>();
            services.AddSingleton<IPAReportService, PAReportService>();
            services.AddScoped<IExportPAReportService, ExportPAReportService>();
            services.AddSingleton<INominationReportService, NominationReportService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IPMSettingService, PMSettingService>();
            services.AddSingleton<IMasterReportService, MasterReportService>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddSingleton<ISkypeNotiService, SkypeNotiService>();
            services.AddSingleton<IPRTrackingService, PRTrackingService>();
            services.AddSingleton<IProbationService, ProbationService>();
            services.AddScoped<IProbationReportService, ProbationReportService>();
            services.AddSingleton<IPRReactionService, PRReactionService>();
            services.AddSingleton<IPRFeedbackService, PRFeedbackService>();
            services.AddSingleton<IPRFeedbackReactionService, PRFeedbackReactionService>();
        }
    }
}