namespace NexGoal.AutoService.Criterias.Probation
{
    using Fanex.Data.Repository;

    public class GetSettingToSendNotificationCriteria : CriteriaBase
    {
        public int ReceiveType { get; set; }

        public override string GetSettingKey() => "Probation_GetSettingToSendNotification";

        public override bool IsValid() => true;
    }
}