namespace NexGoal.Criteria.EmployeeNomination
{
    using Fanex.Data.Repository;

    public class GetNominationsByPeriodIdCriteria : CriteriaBase
    {
        public int NominationPeriodId { get; set; }

        public override string GetSettingKey() => "Nomination_GetByPeriodId";

        public override bool IsValid() => NominationPeriodId > 0;
    }
}