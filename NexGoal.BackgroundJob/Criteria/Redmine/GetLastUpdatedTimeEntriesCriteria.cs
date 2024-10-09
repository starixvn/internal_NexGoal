namespace NexGoal.BackgroundJob.Criteria.Redmine
{
    using Fanex.Data.Repository;

    public class GetLastUpdatedTimeEntriesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "TimeEntries_GetLastUpdated";

        public override bool IsValid() => true;
    }
}