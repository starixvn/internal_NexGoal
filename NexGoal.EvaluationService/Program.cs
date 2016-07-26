namespace NexGoal.AutoGenerateEvaluation
{
    using Fanex.Data.Configuration;

    public class Program
    {
        public static void Main(string[] args)
        {
            DbSettingProviderManager.Current.Start();
            var service = new GeneratedEPAService();
            service.Run();
        }
    }
}