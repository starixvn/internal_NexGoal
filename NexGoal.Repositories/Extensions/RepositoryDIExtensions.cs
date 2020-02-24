namespace NexGoal.Repositories.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using NexGoal.Repositories.Interfaces;

    public static class RepositoryDIExtensions
    {
        public static void ResolveRepositoryDI(this IServiceCollection services)
        {
            services.AddScoped<IDisciplineRepository, DisciplineRepository>();
            services.AddScoped<IPARepository, PARepository>();
            services.AddScoped<IPAFocusRepository, PAFocusRepository>();
            services.AddScoped<IEmployeeNominationRepository, EmployeeNominationRepository>();
            services.AddScoped<IPAReportRepository, PAReportRepository>();
            services.AddScoped<IPerformanceRecordRepository, PerformanceRecordRepository>();
            services.AddScoped<IPRTrackingRepository, PRTrackingRepository>();
            services.AddScoped<IProbationRepository, ProbationRepository>();
            services.AddScoped<IPRReactionRepository, PRReactionRepository>();
            services.AddScoped<IPRFeedbackReactionRepository, PRFeedbackReactionRepository>();
            services.AddScoped<IPRFeedbackRepository, PRFeedbackRepository>();
            services.AddScoped<IMasterReportRepository, MasterReportRepository>();
        }
    }
}