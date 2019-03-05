namespace NexGoal.Criteria.EmployeeNomination
{
    using NexGoal.Core.Criteria;
    public class EditEmployeeNominationCriteria : CommandBaseCriteria
    {
        public int NominationPeriodId { get; set; }

        public int NomineeId { get; set; }

        public int NominatorId { get; set; }

        public string Comment { get; set; }

        public int PerformanceRecordId { get; set; }

        public override string GetSettingKey() => "Nomination_Update";

#pragma warning disable S1067 // Expressions should not be too complex
        public override bool IsValid() => NominationPeriodId > 0 && NomineeId > 0 && NominatorId > 0 && PerformanceRecordId > 0;
#pragma warning restore S1067 // Expressions should not be too complex
    }
}