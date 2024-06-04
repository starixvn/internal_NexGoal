namespace NexGoal.Models.Auth
{
    using System.Collections.Generic;
    public class AccessSettings
    {
        public List<string> IPRanges { get; set; }
        public List<DateRange> DateRanges { get; set; }
    }
}
