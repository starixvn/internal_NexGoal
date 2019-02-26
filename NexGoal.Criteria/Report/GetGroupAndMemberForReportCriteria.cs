namespace NexGoal.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetGroupAndMemberForReportCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public override string GetSettingKey() => "PAReport_GetGroupAndMember";

        public override bool IsValid() => ActorId > 0;
    }
}