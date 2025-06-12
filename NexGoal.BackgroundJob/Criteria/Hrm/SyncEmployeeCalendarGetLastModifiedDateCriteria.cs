namespace NexGoal.BackgroundJob.Criteria.Hrm
{
    using Fanex.Data.Repository;

    public class SyncEmployeeCalendarGetLastModifiedDateCriteria : CriteriaBase
    {

        public override string GetSettingKey() => "SyncEmployeeCalendar_GetLastModifiedDate";

        public override bool IsValid() => true;
    }
} 