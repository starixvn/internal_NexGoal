namespace NexGoal.BackgroundJob.Criterias.Redmine
{
    using Fanex.Data.Repository;

    public class GetLastUpdatedUserCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Users_GetLastUpdated";

        public override bool IsValid() => true;
    }
}