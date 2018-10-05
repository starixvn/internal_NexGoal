namespace NexGoal.Criteria.PerformanceRecord
{
    using Fanex.Data.Repository;

    public class GetCreatorCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_GetCreator";

        public override bool IsValid() => EmployeeId > 0;
    }
}