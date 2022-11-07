namespace NexGoal.Website
{
    using Hangfire.Annotations;
    using Hangfire.Dashboard;

    public class HangfireAuthorization : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}