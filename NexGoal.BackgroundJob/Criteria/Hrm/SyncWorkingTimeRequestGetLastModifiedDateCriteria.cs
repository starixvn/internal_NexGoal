namespace NexGoal.BackgroundJob.Criteria.Hrm
{
    using Fanex.Data.Repository;

    public class SyncWorkingTimeRequestGetLastModifiedDateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SyncWorkingTimeRequest_GetLastModifiedDate";

        public override bool IsValid() => true;
    }
} 