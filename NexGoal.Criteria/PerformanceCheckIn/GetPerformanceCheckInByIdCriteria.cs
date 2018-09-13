namespace NexGoal.Criteria.PerformanceCheckIn
{
    using Fanex.Data.Repository;

    public class GetPerformanceCheckInByIdCriteria : CriteriaBase
    {
        public int PerformanceRecordId { get; set; }
        public int CreatorId { get; set; }

        public override string GetSettingKey() => "PerformanceCheckIn_GetById";

        public override bool IsValid() => PerformanceRecordId > 0 && CreatorId > 0;
    }
}