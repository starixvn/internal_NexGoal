namespace NexGoal.Repositories.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using NexGoal.Repositories.Interfaces;

    public static class RepositoryDIExtensions
    {
        public static void ResolveRepositoryDI(this IServiceCollection services)
        {
            services.AddSingleton<IDisciplineRepository, DisciplineRepository>();
            services.AddSingleton<IPARepository, PARepository>();
            services.AddSingleton<IPAFocusRepository, PAFocusRepository>();
            services.AddSingleton<IEmployeeNominationRepository, EmployeeNominationRepository>();
            services.AddSingleton<IPAReportRepository, PAReportRepository>();
            services.AddSingleton<IPerformanceRecordRepository, PerformanceRecordRepository>();
            services.AddSingleton<IPRTrackingRepository, PRTrackingRepository>();
            services.AddSingleton<IProbationRepository, ProbationRepository>();
            services.AddSingleton<IPRReactionRepository, PRReactionRepository>();
            services.AddSingleton<IPRFeedbackReactionRepository, PRFeedbackReactionRepository>();
            services.AddSingleton<IPRFeedbackRepository, PRFeedbackRepository>();
            services.AddSingleton<IMasterReportRepository, MasterReportRepository>();
        }
    }
}