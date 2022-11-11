namespace NexGoal.Criteria.EmployeeNomination
{
    using Fanex.Data.Repository;

    public class GetNominationsByPeriodIdsEoyCriteria : CriteriaBase
    {
        public string NominationPeriodIds { get; set; }

        public int? StatusId { get; set; }

        public override string GetSettingKey() => "Nomination_GetByPeriodIds_Eoy";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(NominationPeriodIds);
    }
}
