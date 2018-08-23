namespace NexGoal.Criteria.PerformanceCheckIn
{
    using Fanex.Data.Repository;

    public class GetCreatorByEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "PerformanceCheckIn_GetCreatorByEmployee";

        public override bool IsValid() => EmployeeId > 0;
    }
}