namespace NexGoal.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetAnnouncementCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Announcement_Get";

        public override bool IsValid() => EmployeeId > 0;
    }
}