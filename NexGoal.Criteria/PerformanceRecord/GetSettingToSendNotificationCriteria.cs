namespace NexGoal.Criteria.PerformanceRecord
{
    using Fanex.Data.Repository;

    public class GetSettingToSendNotificationCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public string PerformanceRecordIds { get; set; }
        public int ReceiveType { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_GetSettingToSendNotification";

        public override bool IsValid() => ActorId > 0 && !string.IsNullOrWhiteSpace(PerformanceRecordIds);
    }
}