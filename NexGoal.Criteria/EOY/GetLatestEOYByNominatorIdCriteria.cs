namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetLatestEOYByNominatorIdCriteria : CriteriaBase
    {
        public int NominatorId { get; set; }

        public override string GetSettingKey()
        {
            return "EOY_GetLatestNominationByNominatorId";
        }

        public override bool IsValid()
        {
            return NominatorId > 0;
        }
    }
}