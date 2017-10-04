namespace NexGoal.Criteria.PAFocus
{
    using Fanex.Data.Repository;

    public class GetProcessSettingToSendEmailCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "PAFocus_GetProcessSettingToSendEmail";

        public override bool IsValid() => true;
    }
}