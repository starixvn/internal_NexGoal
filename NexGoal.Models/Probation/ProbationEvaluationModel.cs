namespace NexGoal.Models.Probation
{
    using System.Collections.Generic;

    public class ProbationEvaluationModel
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FulllName { get; set; }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public bool? IsQualified { get; set; }

        public int? FinalScore { get; set; }

        public bool CanEdit { get; set; }

        public IEnumerable<ProbationSettingModel> Settings { get; set; }

        public int? CurrentProcessId { get; set; }
    }
}