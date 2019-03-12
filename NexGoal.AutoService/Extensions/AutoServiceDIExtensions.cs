namespace NexGoal.AutoService.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using NexGoal.AutoService.Services;
    public static class AutoServiceDIExtensions
    {
        public static void ResolveAutoServiceDI(this IServiceCollection services)
        {
            services.AddTransient<IPeriodService, PeriodService>();
            services.AddTransient<IPAService, PAService>();
            services.AddTransient<IPAFocusService, PAFocusService>();
            services.AddTransient<INominationService, NominationService>();
            services.AddTransient<IProbationService, ProbationService>();
        }
    }
}