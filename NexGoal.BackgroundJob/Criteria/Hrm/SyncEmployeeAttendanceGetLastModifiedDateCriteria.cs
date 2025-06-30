namespace NexGoal.BackgroundJob.Criteria.Hrm
{
    using Fanex.Data.Repository;

    public class SyncEmployeeAttendanceGetLastModifiedDateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SyncEmployeeAttendance_GetLastModifiedDate";

        public override bool IsValid() => true;
    }
}