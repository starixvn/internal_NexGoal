namespace NexGoal.Criteria.PARecord
{
    using Fanex.Data.Repository;

    public class GetMemberPerformanceAppraisalResult : CriteriaBase
    {
        public int ActorId { get; set; }
        public int? PeriodId { get; set; }
        public int? EmployeeId { get; set; }

        public override string GetSettingKey() => "PARecord_GetMemberPerformanceAppraisalResult";

        public override bool IsValid() => ActorId > 0;
    }
}