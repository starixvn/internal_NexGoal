namespace NexGoal.Criteria.EmployeeNomination
{
    using Fanex.Data.Repository;

    public class GetMemberNominationResultCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int? Year { get; set; }
        public int? EmployeeId { get; set; }

        public override string GetSettingKey() => "Nomination_GetMemberNominationResult";

        public override bool IsValid() => ActorId > 0;
    }
}