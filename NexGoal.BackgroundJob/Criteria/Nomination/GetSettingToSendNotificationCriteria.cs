namespace NexGoal.BackgroundJob.Criteria.Nomination
{
    using Fanex.Data.Repository;

    public class GetSettingToSendNotificationCriteria : CriteriaBase
    {
        public byte ReceiveType { get; set; }

        public override string GetSettingKey() => "Auto_NominationPeriod_GetSettingToSendNotification";

        public override bool IsValid() => true;
    }
}