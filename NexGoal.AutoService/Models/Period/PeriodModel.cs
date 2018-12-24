namespace NexGoal.AutoService.Models.Period
{
    using System;

    public class PeriodModel
    {
        public int PeriodId { get; set; }
        public string Name { get; set; }
        public DateTime FocusStartDate { get; set; }
        public DateTime FocusEndDate { get; set; }
    }
}