namespace NexGoal.Criteria.NominationReport
{
    using Fanex.Data.Repository;

    public class GetNominationsCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public string NominationPeriodIds { get; set; }

        public string GroupIds { get; set; }

        public string EmployeeIds { get; set; }

        public int? StatusId { get; set; }

        public override string GetSettingKey() => "NominationReport_Get";

        public override bool IsValid() => ActorId > 0;
    }
}