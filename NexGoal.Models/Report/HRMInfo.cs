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
    }
}