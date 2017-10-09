namespace NexGoal.Model.Dashboard
{
    using System;

    public class EPAInfoModel
    {
        public int PeriodId { get; set; }

        public string PeriodName { get; set; }

        public int ProcessId { get; set; }

        public int Members { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}