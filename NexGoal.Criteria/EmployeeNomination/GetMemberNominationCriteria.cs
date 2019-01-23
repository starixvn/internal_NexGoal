namespace NexGoal.Criteria.EmployeeNomination
{
    using Fanex.Data.Repository;

    public class GetMemberNominationCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public override string GetSettingKey() => "Nomination_GetMemberNomination";

        public override bool IsValid() => ActorId > 0;
    }
}