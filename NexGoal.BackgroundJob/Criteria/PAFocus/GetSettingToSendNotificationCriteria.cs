namespace NexGoal.BackgroundJob.Criteria.PAFocus
{
    using Fanex.Data.Repository;

    public class GetSettingToSendNotificationCriteria : CriteriaBase
    {
        public byte ReceiveType { get; set; }

        public override string GetSettingKey() => "Auto_PAFocus_GetSettingToSendNotification";

        public override bool IsValid() => true;
    }
}