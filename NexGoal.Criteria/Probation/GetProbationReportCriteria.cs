namespace NexGoal.Criteria.Probation
{
    using Fanex.Data.Repository;
    using System;

    public class GetProbationReportCriteria : CriteriaBase
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string EmployeeIds { get; set; }

        public bool? IsQualifiedByEvaluator { get; set; }

        public bool? IsQualified { get; set; }

        public int ActorId { get; set; }

        public override string GetSettingKey() => "ProbationReport_Get";

        public override bool IsValid() => ActorId > 0;
    }
}