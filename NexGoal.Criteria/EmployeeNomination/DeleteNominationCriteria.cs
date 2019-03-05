namespace NexGoal.Criteria.EmployeeNomination
{
    using NexGoal.Core.Criteria;

    public class DeleteNominationCriteria : CommandBaseCriteria
    {
        public int PerformanceRecordId { get; set; }

        public int DeletedById { get; set; }

        public override string GetSettingKey() => "Nomination_Delete";

        public override bool IsValid() => PerformanceRecordId > 0 && DeletedById > 0;
    }
}