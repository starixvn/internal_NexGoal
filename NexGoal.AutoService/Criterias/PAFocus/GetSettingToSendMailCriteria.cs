namespace NexGoal.AutoService.Criterias.PAFocus
{
    using Fanex.Data.Repository;

    public class GetSettingToSendMailCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "PAFocus_GetSettingToSendMail";

        public override bool IsValid() => true;
    }
}