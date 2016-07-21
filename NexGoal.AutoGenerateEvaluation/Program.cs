namespace NexGoal.AutoGenerateEvaluation
{
    using System.ServiceProcess;
    using Fanex.Data.Configuration;

    public static class Program
    {
        public static void Main()
        {
            DbSettingProviderManager.Current.Start();
            log4net.Config.XmlConfigurator.Configure();

            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[]
            {
                new AutoService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}