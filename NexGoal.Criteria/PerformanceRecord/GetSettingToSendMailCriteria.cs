namespace NexGoal.Criteria.PerformanceRecord
{
    using Fanex.Data.Repository;

    public class GetSettingToSendMailCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public string PerformanceRecordIds { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_GetSettingToSendMail";

        public override bool IsValid() => ActorId > 0 && !string.IsNullOrWhiteSpace(PerformanceRecordIds);
    }
}