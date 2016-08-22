namespace NexGoal.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetMemberByEvaluatorOrReviewerCriteria : CriteriaBase
    {
        public int ProcessId { get; set; }

        public int ActorUserId { get; set; }

        public string GroupId { get; set; }

        public string PeriodIds { get; set; }

        public override bool IsValid()
        {
            return ActorUserId > 0 && ProcessId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_GetMember";
        }
    }
}