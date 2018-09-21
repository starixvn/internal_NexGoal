namespace NexGoal.Criteria.EmployeeNomination
{
    using Fanex.Data.Repository;

    public class GetNominationPeriodsForNominatorCriteria : CriteriaBase
    {
        public int NominatorId { get; set; }

        public override string GetSettingKey() => "Nomination_GetPeriodsForNominator";

        public override bool IsValid() => NominatorId > 0;
    }
}