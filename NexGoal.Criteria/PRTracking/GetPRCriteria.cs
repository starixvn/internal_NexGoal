namespace NexGoal.Criteria.PRTracking
{
    using Fanex.Data.Repository;

    public class GetPRCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public string EmployeeIds { get; set; }

        public int? NoPR { get; set; }

        public override string GetSettingKey() => "PRTracking_Get";

        public override bool IsValid() => ActorId > 0;
    }
}