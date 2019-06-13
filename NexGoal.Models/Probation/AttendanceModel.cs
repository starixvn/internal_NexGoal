namespace NexGoal.Models.Probation
{
    public class AttendanceModel
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public int MissingCheckInCheckOut { get; set; }

        public int LateIn { get; set; }

        public int EarlyOut { get; set; }

        public int UnpaidLeave { get; set; }

        public int UnplanAnnualLeave { get; set; }

        public float Overtime { get; set; }

        public float AverageWorkingHours { get; set; }
    }
}