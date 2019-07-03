namespace NexGoal.Models.Probation
{
    using System;

    public class GetProbationReportModel
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string EmployeeIds { get; set; }

        public bool? IsQualifiedByEvaluator { get; set; }

        public bool? IsQualified { get; set; }

        public int ActorId { get; set; }
    }
}