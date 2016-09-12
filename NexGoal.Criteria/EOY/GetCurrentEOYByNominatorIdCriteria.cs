namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetCurrentEOYByNominatorIdCriteria : CriteriaBase
    {
        public int NominatorId { get; set; }

        public override string GetSettingKey()
        {
            return "EOY_GetCurrentNominationByNominatorId";
        }

        public override bool IsValid()
        {
            return NominatorId > 0;
        }
    }
}