namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetCurrentEOQByNominatorIdCriteria : CriteriaBase
    {
        public int NominatorId { get; set; }

        public override string GetSettingKey()
        {
            return "EOQ_GetCurrentNominationByNominatorId";
        }

        public override bool IsValid()
        {
            return NominatorId > 0;
        }
    }
}