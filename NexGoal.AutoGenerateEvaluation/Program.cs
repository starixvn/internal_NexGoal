namespace NexGoal.AutoGenerateEvaluation
{
    using Fanex.Data.Configuration;
    using System;
    using System.Reflection;
    using System.ServiceProcess;

    public static class Program
    {
        public static void Main(string[] args)
        {
            DbSettingProviderManager.Current.Start();

            var servicesToRun = new ServiceBase[]
            {
                new AutoService()
            };

            if (Environment.UserInteractive)
            {
                var type = typeof(ServiceBase);
                const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var method = type.GetMethod("OnStart", Flags);
                foreach (var service in servicesToRun)
                {
                    method.Invoke(service, new object[] { args });
                }

                Console.WriteLine("Press any key to exit!");
                Console.ReadLine();

                foreach (var service in servicesToRun)
                {
                    service.Stop();
                }
            }
            else
            {
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}