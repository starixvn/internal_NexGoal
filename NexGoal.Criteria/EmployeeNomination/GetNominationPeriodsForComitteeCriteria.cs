namespace NexGoal.Criteria.EmployeeNomination
{
    using Fanex.Data.Repository;

    public class GetNominationPeriodsForComitteeCriteria : CriteriaBase
    {
        public int CommitteeId { get; set; }

        public override string GetSettingKey() => "Nomination_GetPeriodsForCommittee";

        public override bool IsValid() => CommitteeId > 0;
    }
}