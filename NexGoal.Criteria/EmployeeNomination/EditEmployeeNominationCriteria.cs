namespace NexGoal.Criteria.EmployeeNomination
{
    public class EditEmployeeNominationCriteria : CommandBaseCriteria
    {
        public int NominationPeriodId { get; set; }

        public int NomineeId { get; set; }

        public int NominatorId { get; set; }

        public string Comment { get; set; }

        public int PerformanceRecordId { get; set; }

        public override string GetSettingKey() => "Nomination_Update";

        public override bool IsValid() => NominationPeriodId > 0 && NomineeId > 0 && NominatorId > 0 && PerformanceRecordId > 0;
    }
}