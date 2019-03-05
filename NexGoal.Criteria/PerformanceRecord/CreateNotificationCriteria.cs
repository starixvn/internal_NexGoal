namespace NexGoal.Criteria.PerformanceRecord
{
    using NexGoal.Core.Criteria;

    public class CreateNotificationCriteria : CommandBaseCriteria
    {
        public int ActorId { get; set; }
        public string Notifications { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_InsertNotification";

        public override bool IsValid() => ActorId > 0 && !string.IsNullOrWhiteSpace(Notifications);
    }
}