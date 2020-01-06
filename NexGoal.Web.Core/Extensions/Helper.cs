namespace NexGoal.Web.Core.Extensions
{
    using System;

    public static class Helper
    {
        public static string RenderHour(float hours)
        {
            int hour = (int)hours;
            int minute = (int)Math.Round((hours - hour) * 60);
            return hour > 0
                ? ZeroPrefix(hour) + "h" + ZeroPrefix(minute) + "'"
                : ZeroPrefix(minute) + "'";
        }

        private static string ZeroPrefix(object value)
        {
            const byte TotalWidth = 2;
            return value.ToString().PadLeft(TotalWidth, paddingChar: '0');
        }
    }
}