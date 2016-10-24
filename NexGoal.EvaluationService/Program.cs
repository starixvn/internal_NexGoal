namespace NexGoal.BackgroundService
{
    using Fanex.Data;
    using NexGoal.Core.Utilities;

    public class Program
    {
        public static void Main(string[] args)
        {
            DbSettingProviderManager
            .StartNewSession()
            .UseDefaultConnectionStringProvider()
            .Use(new CustomConnectionStringProvider())
            .UseDefaultDbSettingProvider(
                resourcePath: "~/",
                ignoreRedundantParameters: false,
                enableWatching: true)
            .Run();

            var service = new NexGoalService();
            service.Run();
        }
    }
}