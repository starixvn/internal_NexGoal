namespace NexGoal.BackgroundJob.Criterias.PAFocus
{
    using Fanex.Data.Repository;

    public class GetSettingToSendNotificationCriteria : CriteriaBase
    {
        public int ReceiveType { get; set; }

        public override string GetSettingKey() => "Auto_PAFocus_GetSettingToSendNotification";

        public override bool IsValid() => true;
    }
}