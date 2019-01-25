namespace NexGoal.Repositories.Extensions
{
    using Microsoft.Extensions.DependencyInjection;

    public static class RepositoryDIExtensions
    {
        public static void ResolveRepositoryDI(this IServiceCollection services)
        {
            services.AddScoped<IDisciplineRepository, DisciplineRepository>();
            services.AddScoped<IPARepository, PARepository>();
            services.AddScoped<IPAFocusRepository, PAFocusRepository>();
            services.AddScoped<IEmployeeNominationRepository, EmployeeNominationRepository>();
            services.AddScoped<IPAReportRepository, PAReportRepository>();
        }
    }
}