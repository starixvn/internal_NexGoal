namespace NexGoal.AutoService.Criterias.Nomination
{
    using Fanex.Data.Repository;

    public class GetSettingToSendMailCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NominationPeriod_GetSettingToSendMail";

        public override bool IsValid() => true;
    }
}