namespace NexGoal.AutoService.Criterias.PA
{
    using Fanex.Data.Repository;

    public class GetSettingToSendNotificationCriteria : CriteriaBase
    {
        public int ReceiveType { get; set; }

        public override string GetSettingKey() => "PA_GetSettingToSendNotification";

        public override bool IsValid() => true;
    }
}