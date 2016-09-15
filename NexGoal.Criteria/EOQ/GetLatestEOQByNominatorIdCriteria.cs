namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetLatestEOQByNominatorIdCriteria : CriteriaBase
    {
        public int NominatorId { get; set; }

        public override string GetSettingKey()
        {
            return "EOQ_GetLatestNominationByNominatorId";
        }

        public override bool IsValid()
        {
            return NominatorId > 0;
        }
    }
}