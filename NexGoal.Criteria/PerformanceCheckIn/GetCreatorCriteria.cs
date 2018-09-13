namespace NexGoal.Criteria.PerformanceCheckIn
{
    using Fanex.Data.Repository;

    public class GetCreatorCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "PerformanceCheckIn_GetCreator";

        public override bool IsValid() => EmployeeId > 0;
    }
}