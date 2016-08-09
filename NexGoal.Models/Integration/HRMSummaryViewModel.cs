namespace NexGoal.Model.Integration
{
    public class HRMSummaryViewModel
    {
        public int EmployeeId { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public int NumberMissingCheckInOut { get; set; }

        public int NumberLateIn { get; set; }

        public int NumberEarlyOut { get; set; }

        public int NumberUnpaidLeave { get; set; }

        public int NumberUnplannedAnnualLeave { get; set; }

        public float AverageWorkingHours { get; set; }

        public int NumberOvertime { get; set; }

        public int TOEICScore { get; set; }
    }
}
