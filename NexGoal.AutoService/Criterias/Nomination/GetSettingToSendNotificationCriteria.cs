namespace NexGoal.AutoService.Criterias.Nomination
{
    using Fanex.Data.Repository;

    public class GetSettingToSendNotificationCriteria : CriteriaBase
    {
        public int ReceiveType { get; set; }

        public override string GetSettingKey() => "NominationPeriod_GetSettingToSendNotification";

        public override bool IsValid() => true;
    }
}