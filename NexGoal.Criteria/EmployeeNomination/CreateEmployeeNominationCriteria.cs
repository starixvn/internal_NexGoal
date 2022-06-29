namespace NexGoal.Criteria.EmployeeNomination
{
    using NexGoal.Core.Criteria;

    public class CreateEmployeeNominationCriteria : CommandBaseCriteria
    {
        public int NominationPeriodId { get; set; }

        public int NomineeId { get; set; }

        public int NominatorId { get; set; }

        public int PerformanceRecordId { get; set; }

        public string KpiList { get; set; }

        public override string GetSettingKey() => "Nomination_Insert";

        public override bool IsValid() => NominationPeriodId > 0 && NomineeId > 0 && NominatorId > 0;
    }
}