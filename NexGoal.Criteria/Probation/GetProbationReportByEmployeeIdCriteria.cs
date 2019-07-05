namespace NexGoal.Criteria.Probation
{
    using Fanex.Data.Repository;
    using System;

    public class GetProbationReportByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int ActorId { get; set; }

        public override string GetSettingKey() => "ProbationReport_GetByEmployeeId";

        public override bool IsValid() => ActorId > 0;
    }
}