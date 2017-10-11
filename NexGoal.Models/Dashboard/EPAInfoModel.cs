namespace NexGoal.Model.Dashboard
{
    using System;

    public class EPAInfoModel
    {
        public int EPAId { get; set; }

        public int PeriodId { get; set; }

        public string PeriodName { get; set; }

        public int TargetTypeId { get; set; }

        public int ProcessId { get; set; }

        public DateTime EndDate { get; set; }

        public int Members { get; set; }

        public int Unfinished { get; set; }
    }
}