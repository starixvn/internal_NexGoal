namespace NexGoal.Criteria.EmployeeNomination
{
    using Fanex.Data.Repository;

    public class GetMemberNominationNewCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public override string GetSettingKey() => "Nomination_GetMemberNomination_New";

        public override bool IsValid() => ActorId > 0;
    }
}
