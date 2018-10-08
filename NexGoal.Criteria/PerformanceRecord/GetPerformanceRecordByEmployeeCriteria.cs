namespace NexGoal.Criteria.PerformanceRecord
{
    using Fanex.Data.Repository;

    public class GetPerformanceRecordByEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_GetByEmployee";

        public override bool IsValid() => EmployeeId > 0;
    }
}