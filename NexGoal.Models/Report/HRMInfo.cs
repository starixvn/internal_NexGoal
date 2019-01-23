using System;

namespace NexGoal.Models.Report
{
    public class HRMInfo
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public int MissingCheckInCheckOut { get; set; }

        public int LateIn { get; set; }

        public int EarlyOut { get; set; }

        public float UnpaidLeave { get; set; }

        public float UnplanedAnnualLeave { get; set; }

        public float AverageWorkingHours { get; set; }

        public string AverageWorkingHourDisplay
        {
            get
            {
                int hour = (int)AverageWorkingHours;
                int minute = (int)Math.Round((AverageWorkingHours - hour) * 60);
                return hour > 0
                    ? ZeroPrefix(hour) + "h" + ZeroPrefix(minute) + "'"
                    : ZeroPrefix(minute) + "'";
            }
        }

        private static string ZeroPrefix(object value)
        {
            return value.ToString().PadLeft(2, '0');
        }
    }
}