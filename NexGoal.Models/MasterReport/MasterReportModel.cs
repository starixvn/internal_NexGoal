namespace NexGoal.Models.MasterReport
{
    using System.Collections.Generic;

    public class MasterReportModel
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string JobTitle { get; set; }

        public int GroupId { get; set; }

        public string OriginalGradeId { get; set; }

        public string OriginalGrade { get; set; }

        public string GradeId { get; set; }

        public string Grade { get; set; }

        public string Contribution { get; set; }

        public string BonusGiven { get; set; }

        public string IsReadyPromotion { get; set; }

        public string CurrentObjective { get; set; }

        public string NextObjective { get; set; }

        public IEnumerable<AwardModel> Awards { get; set; }
    }
}