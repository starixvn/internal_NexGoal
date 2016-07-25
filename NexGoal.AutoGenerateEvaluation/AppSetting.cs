using System;
using System.Configuration;

namespace NexGoal.AutoGenerateEvaluation
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

        public static int TimerInterval
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["TimerInterval"]);
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
