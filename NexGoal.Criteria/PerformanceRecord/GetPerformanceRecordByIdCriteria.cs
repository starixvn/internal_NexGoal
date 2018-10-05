namespace NexGoal.Criteria.PerformanceRecord
{
    using Fanex.Data.Repository;

    public class GetPerformanceRecordByIdCriteria : CriteriaBase
    {
        public int PerformanceRecordId { get; set; }
        public int CreatorId { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_GetById";

        public override bool IsValid() => PerformanceRecordId > 0 && CreatorId > 0;
    }
}