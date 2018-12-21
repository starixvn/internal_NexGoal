namespace NexGoal.Models.Common
{
    using System;
    using System.Collections.Generic;
    using NexGoal.Models.Discipline;

    public class HRMSummaryModel
    {
        public int ToeicScore { get; set; }

        public bool IsPassedToeic { get; set; }

        public DateTime ToeicExamDate { get; set; }

        public int MissingCheckInCheckOut { get; set; }

        public float AverageWorkingDays { get; set; }

        public IEnumerable<DisciplineModel> Disciplines { get; set; }

        public int TotalLateInEarlyOut { get; set; }

        public int SickLeave { get; set; }

        public int UnpaidLeave { get; set; }

        public int UnplanedAnnualLeave { get; set; }

        public float TotalSpentTime { get; set; }

        public float TotalWorkHours { get; set; }

        public string AverageSpentTime
        {
            get
            {
                if (TotalWorkHours > 0)
                {
                    var avarage = Math.Round((TotalSpentTime / TotalWorkHours) * 100, 2);
                    return string.Format("{0}{1}", avarage, "%");
                }

                return "N/A";
            }
        }
    }
}