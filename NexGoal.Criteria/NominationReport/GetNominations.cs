namespace NexGoal.Criteria.NominationReport
{
    using Fanex.Data.Repository;

    public class GetNominationsCriteria : CriteriaBase
    {
        public int? NominationPeriodId { get; set; }

        public string GroupIds { get; set; }

        public int? EmployeeId { get; set; }

        public int? StatusId { get; set; }

        public override string GetSettingKey() => "NominationReport_GetNominations";

        public override bool IsValid() => true;
    }
}