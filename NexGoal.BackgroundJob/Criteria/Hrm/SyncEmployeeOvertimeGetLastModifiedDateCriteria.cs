namespace NexGoal.BackgroundJob.Criteria.Hrm
{
    using Fanex.Data.Repository;

    public class SyncEmployeeOvertimeGetLastModifiedDateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SyncEmployeeOvertime_GetLastModifiedDate";

        public override bool IsValid() => true;
    }
} 