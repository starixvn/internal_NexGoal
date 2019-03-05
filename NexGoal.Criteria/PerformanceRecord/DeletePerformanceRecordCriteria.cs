namespace NexGoal.Criteria.PerformanceRecord
{
    using NexGoal.Core.Criteria;

    public class DeletePerformanceRecordCriteria : CommandBaseCriteria
    {
        public int PerformanceRecordId { get; set; }
        public int ActorId { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_Delete";

        public override bool IsValid() => ActorId > 0;
    }
}