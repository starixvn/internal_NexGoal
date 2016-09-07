using System;
using System.Configuration;

namespace NexGoal.BackgroundService
{
    public static class AppSetting
    {
        public static bool SendMailEndPeriod
        {
            get
            {
                var value = ConfigurationManager.AppSettings["SendMailEndPeriod"];
                return string.IsNullOrEmpty(value) ? false : Convert.ToBoolean(value);
            }
        }

        public static bool SendMailAllWhenPeriodStarted
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailAllWhenPeriodStarted"]);
            }
        }
    }
}
