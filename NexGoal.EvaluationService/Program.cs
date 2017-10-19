namespace NexGoal.BackgroundService
{
    using Fanex.Data;
    using NexGoal.Core.Utilities;

    public static class Program
    {
        public static void Main()
        {
            DbSettingProviderManager
            .StartNewSession()
            .UseDefaultConnectionStringProvider()
            .Use(new CustomConnectionStringProvider())
            .UseDefaultDbSettingProvider(
                resourcePath: "~/",
                enableWatching: true)
            .Run();

            var service = new NexGoalService();
            service.Run();
        }
    }
}