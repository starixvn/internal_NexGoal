namespace NexGoal.Models.MasterReport
{
    using System.Collections.Generic;
    using NexGoal.Models.PARecord;

    public class MasterReportModel
    {
        public int EmployeeId { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }

        public int GroupId { get; set; }
        public string GroupName { get; set; }

        public int PARecordId { get; set; }

        public string OriginalGradeId { get; set; }
        public string OriginalGrade { get; set; }

        public string GradeId { get; set; }
        public string Grade { get; set; }

        public string Comment { get; set; }

        public int? Toeic { get; set; }
        public int ToeicPolicy { get; set; }

        public int? ContributionId { get; set; }
        public string Contribution { get; set; }

        public int? BonusGivenId { get; set; }
        public string BonusGiven { get; set; }

        public string IsReadyPromotion { get; set; }

        public string CurrentObjective { get; set; }
        public string NextObjective { get; set; }

        public int WorkingDay { get; set; }
        public float ActualWorkingDay { get; set; }
        public float TotalSpentTime { get; set; }

        public bool IsGroupLeader { get; set; }

        public IEnumerable<AwardModel> Awards { get; set; } = new List<AwardModel>();
        public IEnumerable<PAProcessModel> Processes { get; set; } = new List<PAProcessModel>();
    }
}