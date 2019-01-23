namespace NexGoal.Criteria.PerformanceRecord
{
    using Fanex.Data.Repository;

    public class GetDraftPerformanceRecordByIdCriteria : CriteriaBase
    {
        public int PerformanceRecordId { get; set; }

        public override string GetSettingKey() => "DraftPerformanceRecord_GetById";

        public override bool IsValid() => PerformanceRecordId > 0;
    }
}