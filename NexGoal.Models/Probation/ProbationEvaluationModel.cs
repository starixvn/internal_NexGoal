namespace NexGoal.Models.Probation
{
    using System.Collections.Generic;

    public class ProbationEvaluationModel
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Code { get; set; }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public bool? IsQualified { get; set; }

        public string SelfScore { get; set; }

        public string SelfComment { get; set; }

        public string EvaluatorScore { get; set; }

        public string EvaluatorComment { get; set; }

        public string ReviewerComment { get; set; }

        public bool? IsQualifiedByEvaluator { get; set; }

        public bool CanEdit { get; set; }

        public IEnumerable<ProbationSettingModel> Settings { get; set; }

        public IEnumerable<int> ProcessIds { get; set; }
    }
}