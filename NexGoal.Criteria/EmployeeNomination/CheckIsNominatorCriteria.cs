namespace NexGoal.Criteria.EmployeeNomination
{
    using Fanex.Data.Repository;

    public class CheckIsNominatorCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public int? MemberId { get; set; }

        public override string GetSettingKey() => "Nomination_CheckIsNominator";

        public override bool IsValid() => ActorId > 0;
    }
}