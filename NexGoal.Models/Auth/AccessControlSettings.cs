namespace NexGoal.Models.Auth
{
    using System.Collections.Generic;

    public class AccessControlSettings
    {
        public List<string> IPRanges { get; set; }
        public List<PublicDateRange> PublicDateRanges { get; set; }
    }

    public class PublicDateRange
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
