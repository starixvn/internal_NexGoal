namespace NexGoal.BackgroundJob.Criteria.Hrm
{
    using Fanex.Data.Repository;

    public class SyncTOEICGetLastModifiedDateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SyncTOEIC_GetLastModifiedDate";

        public override bool IsValid() => true;
    }
} 