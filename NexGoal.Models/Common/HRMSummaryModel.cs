namespace NexGoal.Models.Common
{
    using System;

    public class HRMSummaryModel
    {
        public int ToeicScore { get; set; }

        public DateTime ToeicExamDate { get; set; }

        public int MissingCheckInCheckOut { get; set; }

        public int Incident { get; set; }

        public int TotalLateInEarlyOut { get; set; }

        public int UnpaidLeave { get; set; }

        public int UnplanedAnnualLeave { get; set; }
    }
}