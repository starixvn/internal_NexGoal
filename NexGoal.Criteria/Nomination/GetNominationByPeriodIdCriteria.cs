namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetNominationByPeriodIdCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int NomineeId { get; set; }

        public override string GetSettingKey()
        {
            return "EOQYNomination_GetByPeriodId";
        }

        public override bool IsValid()
        {
            return PeriodId > 0 && NomineeId > 0;
        }
    }
}