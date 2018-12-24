namespace NexGoal.AutoService.Criterias.PA
{
    using Fanex.Data.Repository;

    public class GetSettingToSendMailCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "PARecord_GetSettingToSendMail";

        public override bool IsValid() => true;
    }
}