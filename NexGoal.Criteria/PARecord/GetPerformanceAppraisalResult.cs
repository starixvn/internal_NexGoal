namespace NexGoal.Criteria.PARecord
{
    using Fanex.Data.Repository;

    public class GetPerformanceAppraisalResult : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "PARecord_GetPerformanceAppraisalResult";

        public override bool IsValid() => EmployeeId > 0;
    }
}