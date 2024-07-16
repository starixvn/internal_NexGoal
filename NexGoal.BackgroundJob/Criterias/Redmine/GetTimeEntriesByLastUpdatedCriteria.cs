namespace NexGoal.BackgroundJob.Criterias.Redmine
{
    using System;
    using Fanex.Data.Repository;

    public class GetTimeEntriesByLastUpdatedCriteria : CriteriaBase
    {
        public DateTime LastUpdated { get; set; }

        public override string GetSettingKey() => "Sync_TimeEntries_GetByLastUpdated";

        public override bool IsValid() => true;
    }
}