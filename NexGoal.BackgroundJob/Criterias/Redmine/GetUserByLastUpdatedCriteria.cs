namespace NexGoal.BackgroundJob.Criterias.Redmine
{
    using System;
    using Fanex.Data.Repository;

    public class GetUserByLastUpdatedCriteria : CriteriaBase
    {
        public DateTime LastUpdated { get; set; }

        public override string GetSettingKey() => "Sync_Users_GetByLastUpdated";

        public override bool IsValid() => true;
    }
}