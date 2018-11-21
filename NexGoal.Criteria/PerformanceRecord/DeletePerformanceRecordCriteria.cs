namespace NexGoal.Criteria.PerformanceRecord
{
    public class DeletePerformanceRecordCriteria : CommandBaseCriteria
    {
        public int PerformanceRecordId { get; set; }
        public int ActorId { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_Delete";

        public override bool IsValid() => PerformanceRecordId > 0 && ActorId > 0;
    }
}