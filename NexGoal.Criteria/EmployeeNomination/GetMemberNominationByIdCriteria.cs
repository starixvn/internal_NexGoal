namespace NexGoal.Criteria.EmployeeNomination
{
    using Fanex.Data.Repository;

    public class GetMemberNominationByIdCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public int MemberId { get; set; }

        public override string GetSettingKey() => "Nomination_GetMemberNominationById";

        public override bool IsValid() => ActorId > 0 && MemberId > 0;
    }
}