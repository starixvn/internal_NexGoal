namespace NexGoal.Criteria.EOQ
{
    using Fanex.Data.Repository;

    public class GetEOQByIdCriteria : CriteriaBase
    {
        public int EOQId { get; set; }

        public override string GetSettingKey() => "EOQ_GetById";

        public override bool IsValid() => EOQId > 0;
    }
}