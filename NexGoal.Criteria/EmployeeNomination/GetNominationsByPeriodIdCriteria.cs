namespace NexGoal.Criteria.EmployeeNomination
{
    using Fanex.Data.Repository;

    public class GetNominationsByPeriodIdsCriteria : CriteriaBase
    {
        public string NominationPeriodIds { get; set; }

        public override string GetSettingKey() => "Nomination_GetByPeriodIds";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(NominationPeriodIds);
    }
}