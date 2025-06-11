namespace NexGoal.BackgroundJob.Criteria.Hrm
{
    using Fanex.Data.Repository;

    public class SyncEmployeeGetLastModifiedDateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SyncEmployee_GetLastModifiedDate";

        public override bool IsValid() => true;
    }
}