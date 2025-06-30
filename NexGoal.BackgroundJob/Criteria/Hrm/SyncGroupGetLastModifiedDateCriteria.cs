namespace NexGoal.BackgroundJob.Criteria.Hrm
{
    using Fanex.Data.Repository;

    public class SyncGroupGetLastModifiedDateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SyncGroup_GetLastModifiedDate";

        public override bool IsValid() => true;
    }
}