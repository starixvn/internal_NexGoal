namespace NexGoal.BackgroundJob.Models.Period
{
    using System;

    public class PeriodModel
    {
        public int PeriodId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime FocusStartDate { get; set; }
        public DateTime FocusEndDate { get; set; }
    }
}