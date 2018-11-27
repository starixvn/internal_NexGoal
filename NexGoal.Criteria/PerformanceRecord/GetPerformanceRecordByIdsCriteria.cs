namespace NexGoal.Criteria.PerformanceRecord
{
    using Fanex.Data.Repository;

    public class GetPerformanceRecordByIdsCriteria : CriteriaBase
    {
        public string PerformanceRecordIds { get; set; }
        public int CreatorId { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_GetByIds";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(PerformanceRecordIds) && CreatorId > 0;
    }
}