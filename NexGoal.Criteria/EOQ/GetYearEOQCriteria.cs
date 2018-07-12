namespace NexGoal.Criteria.EOQ
{
    using Fanex.Data.Repository;

    public class GetYearEOQCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "EOQ_GetYear";

        public override bool IsValid() => true;
    }
}