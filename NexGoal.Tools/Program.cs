namespace NexGoal.Tools
{
    using System;
    using System.Windows.Forms;
    using Fanex.Data.Configuration;

    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            DbSettingProviderManager.Current.Start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}