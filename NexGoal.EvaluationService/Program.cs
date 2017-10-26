namespace NexGoal.BackgroundService
{
    using System;
    using Core.Utilities;
    using Fanex.Data;
    using Fanex.Logging;

    public static class Program
    {
        public static void Main()
        {
            try
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
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    LogService.Error(ex.InnerException.Message, ex);
                }
            }
        }
    }
}