namespace NexGoal.Criteria.NominationReport
{
    using Fanex.Data.Repository;

    public class GetNominationsCriteria : CriteriaBase
    {
        public string NominationPeriodIds { get; set; }

        public string GroupIds { get; set; }

        public string EmployeeIds { get; set; }

        public int? StatusId { get; set; }

        public int LoggedEmployeeId { get; set; }

        public override string GetSettingKey() => "NominationReport_GetNominations";

        public override bool IsValid() => LoggedEmployeeId > 0;
    }
}