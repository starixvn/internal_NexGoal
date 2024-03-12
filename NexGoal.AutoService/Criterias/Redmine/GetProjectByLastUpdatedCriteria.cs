namespace NexGoal.AutoService.Criterias.Redmine
{
    using System;
    using Fanex.Data.Repository;

    public class GetProjectByLastUpdatedCriteria : CriteriaBase
    {
        public DateTime LastUpdated { get; set; }

        public override string GetSettingKey() => "Sync_Project_GetByLastUpdated";

        public override bool IsValid() => true;
    }
}