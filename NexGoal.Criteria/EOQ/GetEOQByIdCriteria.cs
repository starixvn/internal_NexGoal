namespace NexGoal.Criteria.EOQ
{
    using Fanex.Data.Repository;

    public class GetEOQByIdCriteria : CriteriaBase
    {
        public int EOQYId { get; set; }

        public override string GetSettingKey() => "EOQ_GetById";

        public override bool IsValid() => EOQYId > 0;
    }
}